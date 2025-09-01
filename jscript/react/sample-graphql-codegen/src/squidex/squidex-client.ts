import { SquidexClient } from '@squidex/squidex'
import { StorageTokenStore } from '@squidex/squidex/wrapper/SquidexClient'
import Environment from '../config/config'
import { ApolloClient, HttpLink, InMemoryCache } from '@apollo/client'
import { SetContextLink } from '@apollo/client/link/context'

const squidexClient = new SquidexClient({
  appName: Environment.appName,
  clientId: Environment.clientId,
  clientSecret: Environment.clientSecret,
  url: Environment.url,
  tokenStore: new StorageTokenStore(sessionStorage, `${Environment.appName}-token-store`),
})

const authLink = new SetContextLink((prevContext) => {
  const accessToken = squidexClient.clientOptions.tokenStore?.get()?.accessToken
  return {
    headers: {
      ...prevContext.headers,
      authorization: accessToken ? `Bearer ${accessToken}` : '',
    },
  }
})

export const useSquidexGraphqlClient = () => {
  return new ApolloClient({
    link: authLink.concat(
      new HttpLink({
        uri: `${Environment.url}/api/content/${Environment.appName}/graphql`,
      }),
    ),
    cache: new InMemoryCache(),
  })
}

export const useSquidexClient = () => squidexClient
