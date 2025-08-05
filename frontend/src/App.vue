<script setup>
import { ref } from 'vue'
import axios from 'axios'

const longUrl = ref('')
const customAlias = ref('')
const shortenedUrl = ref('')
const isLoading = ref(false)
const showResult = ref(false)
const isDarkMode = ref(true)

const toggleTheme = () => {
  isDarkMode.value = !isDarkMode.value
}

const shortenUrl = async () => {
  if (!longUrl.value.trim()) {
    alert('Please enter a URL to shorten')
    return
  }

  isLoading.value = true
  showResult.value = false

  try {
    // Call your backend API through the proxy
    const response = await axios.post('/api/urls/shorten', {
      originalUrl: longUrl.value,
      customShortCode: customAlias.value.trim() || null,
      expirationDate: null
    }, {
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    // The response contains shortenedUrl field
    shortenedUrl.value = response.data.shortenedUrl
    showResult.value = true
  } catch (error) {
    console.error('Error shortening URL:', error)
    if (error.response) {
      // Server responded with error status
      const errorMessage = error.response.data?.error || error.response.data?.message || 'Server error occurred'
      alert(`Failed to shorten URL: ${errorMessage}`)
    } else if (error.request) {
      // Request was made but no response received
      alert('Failed to connect to server. Please make sure the backend is running.')
    } else {
      // Something else happened
      alert('An unexpected error occurred. Please try again.')
    }
  } finally {
    isLoading.value = false
  }
}

const copyToClipboard = async () => {
  try {
    await navigator.clipboard.writeText(shortenedUrl.value)
    alert('Shortened URL copied to clipboard!')
  } catch (error) {
    console.error('Failed to copy to clipboard:', error)
  }
}
</script>

<template>
  <div class="app" :class="{ 'light-mode': !isDarkMode }">
    <!-- Header -->
    <header class="header">
      <div class="logo">
        <span class="logo-text">EasyURL</span>
      </div>
      <div class="header-controls">
        <!-- Theme Toggle -->
        <button @click="toggleTheme" class="theme-toggle" :title="isDarkMode ? 'Switch to light mode' : 'Switch to dark mode'">
          <svg v-if="isDarkMode" viewBox="0 0 24 24" fill="currentColor" class="theme-icon">
            <path d="M12 2.25a.75.75 0 01.75.75v2.25a.75.75 0 01-1.5 0V3a.75.75 0 01.75-.75zM7.5 12a4.5 4.5 0 119 0 4.5 4.5 0 01-9 0zM18.894 6.166a.75.75 0 00-1.06-1.06l-1.591 1.59a.75.75 0 101.06 1.061l1.591-1.59zM21.75 12a.75.75 0 01-.75.75h-2.25a.75.75 0 010-1.5H21a.75.75 0 01.75.75zM17.834 18.894a.75.75 0 001.06-1.06l-1.59-1.591a.75.75 0 10-1.061 1.06l1.59 1.591zM12 18a.75.75 0 01.75.75V21a.75.75 0 01-1.5 0v-2.25A.75.75 0 0112 18zM7.758 17.303a.75.75 0 00-1.061-1.06l-1.591 1.59a.75.75 0 001.06 1.061l1.591-1.59zM6 12a.75.75 0 01-.75.75H3a.75.75 0 010-1.5h2.25A.75.75 0 016 12zM6.697 7.757a.75.75 0 001.06-1.06l-1.59-1.591a.75.75 0 00-1.061 1.06l1.59 1.591z"/>
          </svg>
          <svg v-else viewBox="0 0 24 24" fill="currentColor" class="theme-icon">
            <path d="M9.528 1.718a.75.75 0 01.162.819A8.97 8.97 0 009 6a9 9 0 009 9 8.97 8.97 0 003.463-.69.75.75 0 01.981.98 10.503 10.503 0 01-9.694 6.46c-5.799 0-10.5-4.701-10.5-10.5 0-4.368 2.667-8.112 6.46-9.694a.75.75 0 01.818.162z"/>
          </svg>
        </button>
        <!-- GitHub Link -->
        <a 
          href="https://github.com/temporary26/AMD" 
          target="_blank" 
          rel="noopener noreferrer"
          class="github-link"
        >
          <svg class="github-icon" viewBox="0 0 24 24" fill="currentColor">
            <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z"/>
          </svg>
        </a>
      </div>
    </header>

    <!-- Main Content -->
    <main class="main">
      <div class="hero-section">
        <h1 class="hero-title">Shorten Your URLs Instantly</h1>
        <p class="hero-subtitle">Enter the link to shorten</p>

        <div class="url-shortener">
          <div class="input-group">
            <input
              v-model="longUrl"
              type="url"
              placeholder="Paste your long URL here..."
              class="url-input"
              @keyup.enter="shortenUrl"
              :disabled="isLoading"
            />
            <input
              v-model="customAlias"
              type="text"
              placeholder="Enter custom alias (optional)"
              class="alias-input"
              @keyup.enter="shortenUrl"
              :disabled="isLoading"
            />
            <div v-if="customAlias.trim()" class="alias-preview">
              <span class="preview-text">Your custom URL will be: </span>
              <span class="preview-url">http://localhost:5001/{{ customAlias.trim() }}</span>
            </div>
          </div>
          <div class="button-group">
            <button 
              @click="shortenUrl"
              :disabled="isLoading"
              class="shorten-btn"
            >
              <span v-if="!isLoading">Shorten</span>
              <div v-else class="loading-spinner"></div>
            </button>
          </div>

          <!-- Result Box -->
          <transition name="slide-down">
            <div v-if="showResult && shortenedUrl" class="result-box">
              <div class="result-content">
                <label class="result-label">Your shortened URL:</label>
                <div class="result-url-container">
                  <input 
                    :value="shortenedUrl" 
                    readonly 
                    class="result-url"
                    @click="$event.target.select()"
                  />
                  <button @click="copyToClipboard" class="copy-btn">
                    <svg viewBox="0 0 24 24" fill="currentColor" class="copy-icon">
                      <path d="M16 1H4c-1.1 0-2 .9-2 2v14h2V3h12V1zm3 4H8c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h11c1.1 0 2-.9 2-2V7c0-1.1-.9-2-2-2zm0 16H8V7h11v14z"/>
                    </svg>
                  </button>
                </div>
              </div>
            </div>
          </transition>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Geist:wght@300;400;500;600;700;800;900&display=swap');

:global(*) {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

:global(html) {
  margin: 0;
  padding: 0;
  overflow-x: hidden;
  width: 100%;
  height: 100%;
}

:global(body) {
  margin: 0;
  padding: 0;
  overflow-x: hidden;
  width: 100%;
  min-height: 100vh;
}

:global(#app) {
  margin: 0;
  padding: 0;
  width: 100%;
  min-height: 100vh;
}

* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

.app {
  min-height: 100vh;
  background: #0a0a0a;
  color: #ffffff;
  font-family: 'Geist', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  position: relative;
  overflow-x: hidden;
  margin: 0;
  padding: 0;
  width: 100%;
  left: 0;
  top: 0;
}

.app::before {
  content: '';
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  width: 100vw;
  height: 100vh;
  background: 
    radial-gradient(ellipse 80% 50% at 20% 40%, rgba(163, 230, 53, 0.15) 0%, transparent 50%),
    radial-gradient(ellipse 60% 80% at 80% 60%, rgba(34, 197, 94, 0.1) 0%, transparent 50%),
    radial-gradient(ellipse 100% 60% at 50% 0%, rgba(59, 130, 246, 0.08) 0%, transparent 50%),
    radial-gradient(ellipse 80% 100% at 0% 100%, rgba(168, 85, 247, 0.06) 0%, transparent 50%),
    radial-gradient(ellipse 120% 80% at 100% 20%, rgba(251, 146, 60, 0.05) 0%, transparent 50%);
  z-index: -2;
  margin: 0;
  padding: 0;
}

.app::after {
  content: '';
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  width: 100vw;
  height: 100vh;
  backdrop-filter: blur(80px);
  z-index: -1;
  margin: 0;
  padding: 0;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem 2rem;
  max-width: 1200px;
  margin: 0 auto;
}

.header-controls {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.theme-toggle {
  background: rgba(255, 255, 255, 0.1);
  border: none;
  border-radius: 12px;
  padding: 0.75rem;
  color: #ffffff;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  backdrop-filter: blur(10px);
}

.theme-toggle:hover {
  background: rgba(255, 255, 255, 0.15);
  transform: translateY(-1px);
}

.theme-icon {
  width: 20px;
  height: 20px;
}

.logo-text {
  font-size: 2.2rem;
  font-weight: 800;
  background: linear-gradient(135deg, #a3e635 0%, #84cc16 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: -0.5px;
  font-family: 'Geist', sans-serif;
}

.github-link {
  color: #ffffff;
  transition: color 0.3s ease;
  text-decoration: none;
}

.github-link:hover {
  color: #a3e635;
}

.github-icon {
  width: 32px;
  height: 32px;
}

.main {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 120px);
  padding: 2rem;
}

.hero-section {
  text-align: center;
  max-width: 600px;
  width: 100%;
}

.hero-title {
  font-size: 4.5rem;
  font-weight: 900;
  margin-bottom: 1.5rem;
  line-height: 1.1;
  letter-spacing: -2px;
  background: linear-gradient(135deg, #ffffff 0%, #f1f5f9 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.hero-title::selection {
  background: #a3e635;
  color: #1a1a1a;
}

.hero-subtitle {
  font-size: 1.5rem;
  color: #cbd5e1;
  margin-bottom: 2.5rem;
  font-weight: 400;
  letter-spacing: -0.25px;
}

.url-shortener {
  width: 100%;
}

.input-group {
  display: flex;
  flex-direction: column;
  gap: 0;
  margin-bottom: 1.5rem;
}

.button-group {
  display: flex;
  justify-content: center;
  margin-bottom: 2rem;
}

.url-input {
  flex: 1;
  min-width: 350px;
  padding: 1.5rem 2rem;
  font-size: 1.125rem;
  border: 2px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  color: #ffffff;
  transition: all 0.3s ease;
  outline: none;
  font-weight: 400;
  font-family: 'Geist', sans-serif;
}

.url-input:focus {
  border-color: #a3e635;
  box-shadow: 0 0 0 4px rgba(163, 230, 53, 0.15);
  background: rgba(255, 255, 255, 0.08);
}

.url-input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.url-input::placeholder {
  color: #94a3b8;
  font-weight: 400;
}

.alias-input {
  flex: 1;
  min-width: 300px;
  margin-top: 1rem;
  padding: 1.5rem 2rem;
  font-size: 1.125rem;
  border: 2px solid rgba(255, 255, 255, 0.1);
  border-radius: 16px;
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  color: #ffffff;
  transition: all 0.3s ease;
  outline: none;
  font-weight: 400;
  font-family: 'Geist', sans-serif;
}

.alias-input:focus {
  border-color: #22d3ee;
  box-shadow: 0 0 0 4px rgba(34, 211, 238, 0.15);
  background: rgba(255, 255, 255, 0.08);
}

.alias-input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.alias-input::placeholder {
  color: #94a3b8;
  font-weight: 400;
}

.alias-preview {
  margin-top: 0.75rem;
  padding: 0.75rem 1rem;
  background: rgba(34, 211, 238, 0.1);
  border: 1px solid rgba(34, 211, 238, 0.3);
  border-radius: 12px;
  font-size: 0.875rem;
}

.preview-text {
  color: #94a3b8;
  font-weight: 400;
}

.preview-url {
  color: #22d3ee;
  font-weight: 500;
  font-family: 'Geist', monospace;
}

.shorten-btn {
  padding: 1.5rem 2.5rem;
  font-size: 1.125rem;
  font-weight: 600;
  background: linear-gradient(135deg, #a3e635 0%, #84cc16 100%);
  color: #0a0a0a;
  border: none;
  border-radius: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  min-width: 140px;
  display: flex;
  align-items: center;
  justify-content: center;
  letter-spacing: -0.25px;
  box-shadow: 0 4px 20px rgba(163, 230, 53, 0.2);
  font-family: 'Geist', sans-serif;
}

.shorten-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(163, 230, 53, 0.3);
}

.shorten-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid #1a1a1a;
  border-top: 2px solid transparent;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.result-box {
  background: rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 20px;
  padding: 2.5rem;
  margin-top: 2rem;
}

.result-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.result-label {
  font-size: 1rem;
  color: #cbd5e1;
  font-weight: 500;
  text-align: left;
  letter-spacing: -0.25px;
}

.result-url-container {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.result-url {
  flex: 1;
  padding: 1.25rem 1.5rem;
  font-size: 1.125rem;
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.05);
  color: #a3e635;
  font-weight: 500;
  outline: none;
  cursor: pointer;
  font-family: 'Geist', monospace;
  letter-spacing: -0.25px;
}

.result-url:focus {
  border-color: #a3e635;
}

.copy-btn {
  padding: 1.25rem;
  background: rgba(255, 255, 255, 0.1);
  border: none;
  border-radius: 12px;
  color: #ffffff;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  backdrop-filter: blur(10px);
}

.copy-btn:hover {
  background: #a3e635;
  color: #0a0a0a;
}

.copy-icon {
  width: 20px;
  height: 20px;
}

.slide-down-enter-active {
  transition: all 0.4s ease-out;
}

.slide-down-leave-active {
  transition: all 0.3s ease-in;
}

.slide-down-enter-from {
  opacity: 0;
  transform: translateY(-20px);
}

.slide-down-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

@media (max-width: 768px) {
  .header {
    padding: 1rem;
  }
  
  .hero-title {
    font-size: 3rem;
    letter-spacing: -1px;
  }
  
  .hero-subtitle {
    font-size: 1.25rem;
    margin-bottom: 3rem;
  }
  
  .input-group {
    flex-direction: column;
    gap: 1.5rem;
  }
  
  .url-input {
    min-width: auto;
    padding: 1.25rem 1.5rem;
    font-size: 1rem;
  }
  
  .alias-input {
    min-width: auto;
    padding: 1.25rem 1.5rem;
    font-size: 1rem;
  }
  
  .shorten-btn {
    padding: 1.25rem 2rem;
    font-size: 1rem;
  }
  
  .result-url-container {
    flex-direction: column;
    gap: 1rem;
  }
  
  .copy-btn {
    align-self: stretch;
  }
  
  .result-box {
    padding: 2rem;
  }
}

/* Light Mode Styles */
.app.light-mode {
  background: #ffffff;
  color: #1a1a1a;
}

.app.light-mode::before {
  background: 
    radial-gradient(ellipse 80% 50% at 20% 40%, rgba(163, 230, 53, 0.08) 0%, transparent 50%),
    radial-gradient(ellipse 60% 80% at 80% 60%, rgba(34, 197, 94, 0.06) 0%, transparent 50%),
    radial-gradient(ellipse 100% 60% at 50% 0%, rgba(59, 130, 246, 0.05) 0%, transparent 50%),
    radial-gradient(ellipse 80% 100% at 0% 100%, rgba(168, 85, 247, 0.04) 0%, transparent 50%),
    radial-gradient(ellipse 120% 80% at 100% 20%, rgba(251, 146, 60, 0.03) 0%, transparent 50%);
}

.app.light-mode::after {
  backdrop-filter: blur(40px);
}

.app.light-mode .hero-title {
  background: linear-gradient(135deg, #1a1a1a 0%, #374151 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.app.light-mode .hero-subtitle {
  color: #6b7280;
}

.app.light-mode .url-input {
  background: rgba(0, 0, 0, 0.03);
  border-color: rgba(0, 0, 0, 0.1);
  color: #1a1a1a;
}

.app.light-mode .url-input:focus {
  background: rgba(0, 0, 0, 0.05);
  border-color: #a3e635;
}

.app.light-mode .url-input::placeholder {
  color: #9ca3af;
}

.app.light-mode .alias-input {
  background: rgba(0, 0, 0, 0.03);
  border-color: rgba(0, 0, 0, 0.1);
  color: #1a1a1a;
}

.app.light-mode .alias-input:focus {
  background: rgba(0, 0, 0, 0.05);
  border-color: #22d3ee;
}

.app.light-mode .alias-input::placeholder {
  color: #9ca3af;
}

.app.light-mode .alias-preview {
  background: rgba(34, 211, 238, 0.05);
  border-color: rgba(34, 211, 238, 0.2);
}

.app.light-mode .preview-text {
  color: #6b7280;
}

.app.light-mode .preview-url {
  color: #0891b2;
}

.app.light-mode .result-box {
  background: rgba(0, 0, 0, 0.03);
  border-color: rgba(0, 0, 0, 0.1);
}

.app.light-mode .result-label {
  color: #6b7280;
}

.app.light-mode .result-url {
  background: rgba(0, 0, 0, 0.02);
  border-color: rgba(0, 0, 0, 0.1);
  color: #059669;
}

.app.light-mode .copy-btn {
  background: rgba(0, 0, 0, 0.05);
  color: #6b7280;
}

.app.light-mode .copy-btn:hover {
  background: #a3e635;
  color: #ffffff;
}

.app.light-mode .theme-toggle {
  background: rgba(0, 0, 0, 0.05);
  color: #6b7280;
}

.app.light-mode .theme-toggle:hover {
  background: rgba(0, 0, 0, 0.1);
}

.app.light-mode .github-link {
  color: #6b7280;
}

.app.light-mode .github-link:hover {
  color: #a3e635;
}
</style>
