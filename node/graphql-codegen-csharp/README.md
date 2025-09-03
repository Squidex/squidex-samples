# GraphQL Codegen CSharp

A project that converts schemas into C# POCOs through GraphQL Introspection.
This can be helpful for cases where complex references are pre-resolve via GraphQL, stored as JSON, and parsed at runtime.

## Setup
- node (ts-node)
- TypeScript
- GraphQL
- GraphQL-Codegen

## How to Run
```bash
yarn && yarn codegen
```

# known issues
The c-sharp plugin for graphql-codegen does not return results for union types that link multiple complex schemas of reference types.

## Available Scripts
- `lint`: Runs ESLint for code linting
- `format`: Checks code formatting with Prettier
- `format:fix`: Fixes code formatting with Prettier
- `codegen`: Generates CSharp types from GraphQL schema using GraphQL-Codegen