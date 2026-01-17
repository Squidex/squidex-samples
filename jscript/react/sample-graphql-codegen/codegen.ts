import type { CodegenConfig } from '@graphql-codegen/cli'
import { loadEnv } from 'vite'
import { SquidexClient } from '@squidex/squidex'
import { InMemoryTokenStore } from '@squidex/squidex/wrapper/SquidexClient'

const env = loadEnv(process.env.NODE_ENV || 'development', process.cwd(), '')

const appName = env.VITE_SQUIDEX_APP_NAME
const clientId = env.VITE_SQUIDEX_CLIENT_ID
const clientSecret = env.VITE_SQUIDEX_CLIENT_SECRET
const squidexHost = env.VITE_SQUIDEX_HOST

const url = `${env.VITE_SQUIDEX_HOST}/api/content/${appName}/graphql`
const schema = {}
const client = new SquidexClient({
  appName,
  clientId,
  clientSecret,
  url: squidexHost,
  tokenStore: new InMemoryTokenStore(),
})

const app = await client.apps.getApp()
const tokenStore = client.clientOptions.tokenStore?.get()
console.log(`${app.name} loaded!`)

schema[url] = {
  headers: {
    Authorization: `Bearer ${tokenStore?.accessToken}`,
  },
}

const config: CodegenConfig = {
  overwrite: true,
  schema,
  documents: ['src/**/*.graphql'],
  generates: {
    './src/__generated__/': {
      preset: 'client',
      presetConfig: {
        gqlTagName: 'gql',
      },
      config: {
        useTypeImports: true,
        enumsAsConst: true,
      },
    },
  },
  hooks: {
    afterOneFileWrite: ['prettier --write'],
  },
}

export default config
