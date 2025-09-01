const Config = {
  appName: import.meta.env.VITE_APP_NAME,
  url: import.meta.env.VITE_URL,
  clientId: import.meta.env.VITE_CLIENT_ID,
  clientSecret: import.meta.env.VITE_CLIENT_SECRET,
  isDevelopment: import.meta.env.DEV,
  isProduction: import.meta.env.PROD,
  isLocalhost: import.meta.env.VITE_USER_NODE_ENV === 'localhost',
}

export default Config
