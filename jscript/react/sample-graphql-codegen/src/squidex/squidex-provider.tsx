import { useQuery } from '@apollo/client/react'
import { useSquidexClient } from './squidex-client'
import { useEffect, useState } from 'react'
import { PageQueryDocument } from '../__generated__/graphql'

export default function SquidexProvider() {
  const { loading, data, error } = useQuery(PageQueryDocument)
  const client = useSquidexClient()
  const [appName, setAppName] = useState<string>('')

  if (error) {
    return <h1>Loading Error!</h1>
  }

  useEffect(() => {
    const fetchData = async () => {
      let app = await client.apps.getApp()
      setAppName(app.name)
    }
    fetchData()
  }, [])

  const total = data?.queryPagesContentsWithTotal?.total
  const items = data?.queryPagesContentsWithTotal?.items

  return (
    <>
      <h1>hi! my app is {appName}</h1>
      {loading ? (
        <p>hotel graphql loading...</p>
      ) : (
        <>
          <p>total : {total}</p>
          <ul>
            {items?.map((item) => (
              <li key={item.id}>{item.flatData.title}</li>
            ))}
          </ul>
        </>
      )}
    </>
  )
}
