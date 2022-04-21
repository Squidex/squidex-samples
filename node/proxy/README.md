# Squidex Proxy

![Docker Pulls](https://img.shields.io/docker/pulls/squidex/node-proxy)

This node application works as a small proxy server in front of Squidex for high availability. 

The goal is to make our clients and architecture more resilient in case something goes wrong.

## How to run it locally?

Just install node and run

```
npm i
npm start
```

Then go to `http://localhost:3000/api/content/squidex-website/feature-news`

## How to run it on a server

Just use the docker build: https://hub.docker.com/repository/docker/squidex/node-proxy/general

## How it works?

The proxy forwards the request using the path and query string to a Squidex instance. It also handles authorization and adds the credentials as bearer token to all requests.

When a request returns `HTTP 200` it is cached in redis for 10 minutes and then delivered the next time. The cache key is calculated from all custom Squidex headers (that usually start with `X-`), the path and query string.

Furthermore Squidex returns a list of surrogate keys. These are all keys, such as content ID that form the result.

For each surrogate key this proxy maintains a list in Redis with all cached URLs where the content ID is part of.

When a content item has been changed you can make a request to 

```
http://localhost:3000/cache/purge/key/{surrogate-key}
```

to purge all cache entries.

The surrogate key is calculate as combination of the app id and content id, such as 

```
{APP-ID}--{CONTENT-ID}
```

## How to configure it?

This proxy only has a few environment variables: [config.ts](config.ts)

