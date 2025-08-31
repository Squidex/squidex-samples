import SquidexProvider from './squidex/squidex-provider'
import { ApolloProvider } from '@apollo/client/react'
import { useSquidexGraphqlClient } from './squidex/squidex-client'

function App() {
  const apolloClient = useSquidexGraphqlClient()

  return (
    <>
      <ApolloProvider client={apolloClient}>
        <SquidexProvider />
      </ApolloProvider>
    </>
  )
}

export default App
