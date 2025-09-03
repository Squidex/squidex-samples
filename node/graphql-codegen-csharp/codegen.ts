import type { CodegenConfig } from '@graphql-codegen/cli'
import { SquidexClient } from '@squidex/squidex'
import { InMemoryTokenStore } from '@squidex/squidex/wrapper/SquidexClient'
import dotenv from 'dotenv'
dotenv.config()

const appName = process.env.APP_NAME ?? ''
const clientId = process.env.CLIENT_ID ?? ''
const clientSecret = process.env.CLIENT_SECRET ?? ''
const squidexHost = process.env.URL

const url = `${squidexHost}/api/content/${appName}/graphql`
const schema: any = {}
const client = new SquidexClient({
  appName,
  clientId,
  clientSecret,
  url: squidexHost,
  tokenStore: new InMemoryTokenStore(),
})

schema[url] = {
  headers: {
    Authorization: `Bearer ${await client.getToken()}`,
  },
}

const config: CodegenConfig = {
  overwrite: true,
  schema,
  generates: {
    './output/Squidex.Generated.cs': {
      config: {
        namespaceName: 'Squidex',
        className: 'GraphQL',
        defaultScalarType: 'object',
        skipTypename: true,
        nonOptionalTypename: true,
        useTypeImports: true,
        typesafeOperation: true,
        scalars: {
          Instant: 'DateTime',
        },
      },
      plugins: ['c-sharp'],
    },
  },
}

export default config
