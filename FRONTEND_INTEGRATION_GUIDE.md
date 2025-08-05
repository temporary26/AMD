# Vue.js Frontend Integration Guide

## 🔌 **API Endpoints to Integrate:**

### **Base URL:** `http://localhost:5000` (API Gateway)

#### **1. Create Shortened URL**
```javascript
POST /api/urls/shorten
Content-Type: application/json

{
  "originalUrl": "https://example.com",
  "customShortCode": null,        // Optional: custom short code
  "expirationDate": null          // Optional: ISO date string
}

// Response:
{
  "shortCode": "s8iasUP",
  "originalUrl": "https://example.com",
  "shortenedUrl": "http://localhost:5000/r/s8iasUP",
  "createdAt": "2025-08-05T15:50:04.3784128Z",
  "expirationDate": null,
  "clickCount": 0
}
```

#### **2. Get Original URL**
```javascript
GET /api/urls/{shortCode}

// Response:
{
  "originalUrl": "https://example.com"
}
```

#### **3. Health Check**
```javascript
GET /health/urls

// Response:
{
  "status": "Healthy",
  "service": "URL",
  "timestamp": "2025-08-05T15:49:47.4727619Z"
}
```

## 🎨 **Recommended Vue.js Structure:**

### **Components Needed:**
1. **UrlShortener.vue** - Main form to shorten URLs
2. **UrlResult.vue** - Display shortened URL result
3. **UrlHistory.vue** - List of user's shortened URLs (localStorage)
4. **UrlValidator.vue** - URL validation component
5. **CopyButton.vue** - Copy to clipboard functionality

### **Vue 3 + Composition API Example:**

```vue
<template>
  <div class="url-shortener">
    <div class="container">
      <h1>URL Shortener</h1>
      
      <!-- URL Input Form -->
      <div class="input-section">
        <form @submit.prevent="shortenUrl">
          <div class="input-group">
            <input
              v-model="originalUrl"
              type="url"
              placeholder="Enter your long URL here..."
              required
              class="url-input"
            />
            <button type="submit" :disabled="loading" class="shorten-btn">
              {{ loading ? 'Shortening...' : 'Shorten URL' }}
            </button>
          </div>
          
          <!-- Optional Custom Code -->
          <div class="optional-fields">
            <input
              v-model="customShortCode"
              type="text"
              placeholder="Custom short code (optional)"
              maxlength="20"
              class="custom-code-input"
            />
          </div>
        </form>
      </div>

      <!-- Result Display -->
      <div v-if="result" class="result-section">
        <h3>Your shortened URL:</h3>
        <div class="result-display">
          <input 
            :value="result.shortenedUrl" 
            readonly 
            class="result-input"
            ref="resultInput"
          />
          <button @click="copyToClipboard" class="copy-btn">
            {{ copied ? 'Copied!' : 'Copy' }}
          </button>
        </div>
        <p class="original-url">Original: {{ result.originalUrl }}</p>
      </div>

      <!-- Error Display -->
      <div v-if="error" class="error-section">
        <p class="error-message">{{ error }}</p>
      </div>

      <!-- URL History -->
      <div v-if="urlHistory.length" class="history-section">
        <h3>Recent URLs</h3>
        <div v-for="url in urlHistory" :key="url.shortCode" class="history-item">
          <div class="url-pair">
            <a :href="url.shortenedUrl" target="_blank" class="short-url">
              {{ url.shortenedUrl }}
            </a>
            <span class="arrow">→</span>
            <span class="original-url">{{ url.originalUrl }}</span>
          </div>
          <button @click="copyUrl(url.shortenedUrl)" class="copy-small">Copy</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

// Reactive data
const originalUrl = ref('')
const customShortCode = ref('')
const result = ref(null)
const error = ref('')
const loading = ref(false)
const copied = ref(false)
const urlHistory = ref([])
const resultInput = ref(null)

// API Base URL
const API_BASE = 'http://localhost:5000'

// Load history from localStorage
onMounted(() => {
  const saved = localStorage.getItem('urlHistory')
  if (saved) {
    urlHistory.value = JSON.parse(saved)
  }
})

// Shorten URL function
const shortenUrl = async () => {
  if (!originalUrl.value) return
  
  loading.value = true
  error.value = ''
  result.value = null
  
  try {
    const response = await fetch(`${API_BASE}/api/urls/shorten`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        originalUrl: originalUrl.value,
        customShortCode: customShortCode.value || null,
        expirationDate: null
      })
    })
    
    if (!response.ok) {
      const errorData = await response.text()
      throw new Error(`Failed to shorten URL: ${errorData}`)
    }
    
    const data = await response.json()
    result.value = data
    
    // Add to history
    urlHistory.value.unshift(data)
    if (urlHistory.value.length > 10) {
      urlHistory.value = urlHistory.value.slice(0, 10)
    }
    localStorage.setItem('urlHistory', JSON.stringify(urlHistory.value))
    
    // Reset form
    originalUrl.value = ''
    customShortCode.value = ''
    
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

// Copy to clipboard
const copyToClipboard = async () => {
  try {
    await navigator.clipboard.writeText(result.value.shortenedUrl)
    copied.value = true
    setTimeout(() => {
      copied.value = false
    }, 2000)
  } catch (err) {
    // Fallback for older browsers
    resultInput.value.select()
    document.execCommand('copy')
    copied.value = true
    setTimeout(() => {
      copied.value = false
    }, 2000)
  }
}

// Copy any URL
const copyUrl = async (url) => {
  try {
    await navigator.clipboard.writeText(url)
  } catch (err) {
    console.error('Failed to copy:', err)
  }
}
</script>

<style scoped>
.url-shortener {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.container {
  background: white;
  padding: 2rem;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.2);
  max-width: 600px;
  width: 90%;
}

h1 {
  text-align: center;
  color: #333;
  margin-bottom: 2rem;
}

.input-group {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
}

.url-input {
  flex: 1;
  padding: 1rem;
  border: 2px solid #ddd;
  border-radius: 8px;
  font-size: 1rem;
}

.url-input:focus {
  outline: none;
  border-color: #667eea;
}

.shorten-btn {
  padding: 1rem 2rem;
  background: #667eea;
  color: white;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
}

.shorten-btn:hover:not(:disabled) {
  background: #5a6fd8;
}

.shorten-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.custom-code-input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #ddd;
  border-radius: 8px;
  font-size: 0.9rem;
}

.result-section {
  margin: 2rem 0;
  padding: 1.5rem;
  background: #f8f9fa;
  border-radius: 8px;
}

.result-display {
  display: flex;
  gap: 1rem;
  margin: 1rem 0;
}

.result-input {
  flex: 1;
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  background: white;
}

.copy-btn {
  padding: 0.75rem 1.5rem;
  background: #28a745;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.copy-btn:hover {
  background: #218838;
}

.error-section {
  margin: 1rem 0;
  padding: 1rem;
  background: #f8d7da;
  color: #721c24;
  border-radius: 6px;
}

.history-section {
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 1px solid #ddd;
}

.history-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  margin: 0.5rem 0;
  background: #f8f9fa;
  border-radius: 6px;
}

.url-pair {
  flex: 1;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.short-url {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
}

.arrow {
  color: #666;
}

.original-url {
  color: #666;
  font-size: 0.9rem;
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.copy-small {
  padding: 0.5rem 1rem;
  background: #6c757d;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.8rem;
}
</style>
```

## 📦 **Package.json Dependencies:**

```json
{
  "name": "url-shortener-frontend",
  "version": "1.0.0",
  "dependencies": {
    "vue": "^3.3.0",
    "axios": "^1.5.0"
  },
  "devDependencies": {
    "@vitejs/plugin-vue": "^4.3.0",
    "vite": "^4.4.0"
  }
}
```

## 🚀 **Quick Setup Commands:**

```bash
# Create Vue app
npm create vue@latest url-shortener-frontend
cd url-shortener-frontend

# Install dependencies
npm install axios

# Run development server
npm run dev
```

## 🔧 **Key Integration Points:**

1. **CORS**: Your backend might need CORS configuration
2. **Error Handling**: Handle network errors gracefully
3. **Validation**: Client-side URL validation
4. **Responsive Design**: Mobile-friendly interface
5. **State Management**: Use Pinia if you need global state

## 🎨 **UI/UX Considerations:**

1. **Instant Feedback**: Show loading states
2. **Copy Functionality**: One-click copy to clipboard
3. **URL Preview**: Show shortened URL immediately
4. **History**: Local storage for recent URLs
5. **Validation**: Real-time URL validation
6. **Responsive**: Works on mobile devices

Would you like me to help you set up any specific part of this frontend implementation?
