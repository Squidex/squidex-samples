/// <reference types="vite/client" />

interface ViteTypeOptions {
  strictImportEnv: unknown
}

interface ImportMetaEnv {
  readonly VITE_APP_TITLE: string
  readonly VITE_URL: string
  readonly VITE_APP_NAME: string
  readonly VITE_CLIENT_ID: string
  readonly VITE_CLIENT_SECRET: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
