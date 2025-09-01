/* eslint-disable */
import type { TypedDocumentNode as DocumentNode } from '@graphql-typed-document-node/core';
export type Maybe<T> = T | null;
export type InputMaybe<T> = Maybe<T>;
export type Exact<T extends { [key: string]: unknown }> = { [K in keyof T]: T[K] };
export type MakeOptional<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]?: Maybe<T[SubKey]> };
export type MakeMaybe<T, K extends keyof T> = Omit<T, K> & { [SubKey in K]: Maybe<T[SubKey]> };
export type MakeEmpty<T extends { [key: string]: unknown }, K extends keyof T> = { [_ in K]?: never };
export type Incremental<T> = T | { [P in keyof T]?: P extends ' $fragmentName' | '__typename' ? T[P] : never };
/** All built-in and custom scalars, mapped to their actual values */
export type Scalars = {
  ID: { input: string; output: string; }
  String: { input: string; output: string; }
  Boolean: { input: boolean; output: boolean; }
  Int: { input: number; output: number; }
  Float: { input: number; output: number; }
  /** The `DateTime` scalar type represents a date and time. `DateTime` expects timestamps to be formatted in accordance with the [ISO-8601](https://en.wikipedia.org/wiki/ISO_8601) standard. */
  Instant: { input: any; output: any; }
  /** Unstructured Json object */
  JsonScalar: { input: any; output: any; }
  Long: { input: any; output: any; }
};

export type AllContents = Pages | Posts;

/** The app mutations. */
export type ApplicationMutations = {
  __typename?: 'ApplicationMutations';
  /** Change a pages content. */
  changePagesContent?: Maybe<Pages>;
  /** Change a posts content. */
  changePostsContent?: Maybe<Posts>;
  /** Creates an pages content. */
  createPagesContent?: Maybe<Pages>;
  /** Creates an posts content. */
  createPostsContent?: Maybe<Posts>;
  /** Delete an pages content. */
  deletePagesContent: EntitySavedResultDto;
  /** Delete an posts content. */
  deletePostsContent: EntitySavedResultDto;
  /** Patch an pages content by id. */
  patchPagesContent?: Maybe<Pages>;
  /** Patch an posts content by id. */
  patchPostsContent?: Maybe<Posts>;
  /**
   * Publish a pages content.
   * @deprecated Use 'changePagesContent' instead
   */
  publishPagesContent?: Maybe<Pages>;
  /**
   * Publish a posts content.
   * @deprecated Use 'changePostsContent' instead
   */
  publishPostsContent?: Maybe<Posts>;
  /** Update an pages content by id. */
  updatePagesContent?: Maybe<Pages>;
  /** Update an posts content by id. */
  updatePostsContent?: Maybe<Posts>;
  /** Upsert an pages content by id. */
  upsertPagesContent?: Maybe<Pages>;
  /** Upsert an posts content by id. */
  upsertPostsContent?: Maybe<Posts>;
};


/** The app mutations. */
export type ApplicationMutationsChangePagesContentArgs = {
  dueTime?: InputMaybe<Scalars['Instant']['input']>;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id: Scalars['String']['input'];
  status: Scalars['String']['input'];
};


/** The app mutations. */
export type ApplicationMutationsChangePostsContentArgs = {
  dueTime?: InputMaybe<Scalars['Instant']['input']>;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id: Scalars['String']['input'];
  status: Scalars['String']['input'];
};


/** The app mutations. */
export type ApplicationMutationsCreatePagesContentArgs = {
  data: PagesDataInputDto;
  id?: InputMaybe<Scalars['String']['input']>;
  publish?: InputMaybe<Scalars['Boolean']['input']>;
  status?: InputMaybe<Scalars['String']['input']>;
};


/** The app mutations. */
export type ApplicationMutationsCreatePostsContentArgs = {
  data: PostsDataInputDto;
  id?: InputMaybe<Scalars['String']['input']>;
  publish?: InputMaybe<Scalars['Boolean']['input']>;
  status?: InputMaybe<Scalars['String']['input']>;
};


/** The app mutations. */
export type ApplicationMutationsDeletePagesContentArgs = {
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id: Scalars['String']['input'];
};


/** The app mutations. */
export type ApplicationMutationsDeletePostsContentArgs = {
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id: Scalars['String']['input'];
};


/** The app mutations. */
export type ApplicationMutationsPatchPagesContentArgs = {
  data: PagesDataInputDto;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id?: InputMaybe<Scalars['String']['input']>;
};


/** The app mutations. */
export type ApplicationMutationsPatchPostsContentArgs = {
  data: PostsDataInputDto;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id?: InputMaybe<Scalars['String']['input']>;
};


/** The app mutations. */
export type ApplicationMutationsPublishPagesContentArgs = {
  dueTime?: InputMaybe<Scalars['Instant']['input']>;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id: Scalars['String']['input'];
  status: Scalars['String']['input'];
};


/** The app mutations. */
export type ApplicationMutationsPublishPostsContentArgs = {
  dueTime?: InputMaybe<Scalars['Instant']['input']>;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id: Scalars['String']['input'];
  status: Scalars['String']['input'];
};


/** The app mutations. */
export type ApplicationMutationsUpdatePagesContentArgs = {
  data: PagesDataInputDto;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id?: InputMaybe<Scalars['String']['input']>;
};


/** The app mutations. */
export type ApplicationMutationsUpdatePostsContentArgs = {
  data: PostsDataInputDto;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id?: InputMaybe<Scalars['String']['input']>;
};


/** The app mutations. */
export type ApplicationMutationsUpsertPagesContentArgs = {
  data: PagesDataInputDto;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id: Scalars['String']['input'];
  patch?: InputMaybe<Scalars['Boolean']['input']>;
  publish?: InputMaybe<Scalars['Boolean']['input']>;
  status?: InputMaybe<Scalars['String']['input']>;
};


/** The app mutations. */
export type ApplicationMutationsUpsertPostsContentArgs = {
  data: PostsDataInputDto;
  expectedVersion?: InputMaybe<Scalars['Int']['input']>;
  id: Scalars['String']['input'];
  patch?: InputMaybe<Scalars['Boolean']['input']>;
  publish?: InputMaybe<Scalars['Boolean']['input']>;
  status?: InputMaybe<Scalars['String']['input']>;
};

/** The app queries. */
export type ApplicationQueries = {
  __typename?: 'ApplicationQueries';
  /** Find an asset by id. */
  findAsset?: Maybe<Asset>;
  /** Find an pages content by id. */
  findPagesContent?: Maybe<Pages>;
  /** Find an posts content by id. */
  findPostsContent?: Maybe<Posts>;
  /** Get assets. */
  queryAssets: Array<Asset>;
  /** Get assets and total count. */
  queryAssetsWithTotal: AssetResultDto;
  /** Query content items by IDs across schemeas. */
  queryContentsByIds: Array<AllContents>;
  /** Query pages content items. */
  queryPagesContents?: Maybe<Array<Pages>>;
  /** Query pages content items with total count. */
  queryPagesContentsWithTotal?: Maybe<PagesResultDto>;
  /** Query posts content items. */
  queryPostsContents?: Maybe<Array<Posts>>;
  /** Query posts content items with total count. */
  queryPostsContentsWithTotal?: Maybe<PostsResultDto>;
};


/** The app queries. */
export type ApplicationQueriesFindAssetArgs = {
  id: Scalars['String']['input'];
};


/** The app queries. */
export type ApplicationQueriesFindPagesContentArgs = {
  id: Scalars['String']['input'];
  version?: InputMaybe<Scalars['Int']['input']>;
};


/** The app queries. */
export type ApplicationQueriesFindPostsContentArgs = {
  id: Scalars['String']['input'];
  version?: InputMaybe<Scalars['Int']['input']>;
};


/** The app queries. */
export type ApplicationQueriesQueryAssetsArgs = {
  filter?: InputMaybe<Scalars['String']['input']>;
  orderby?: InputMaybe<Scalars['String']['input']>;
  skip?: InputMaybe<Scalars['Int']['input']>;
  top?: InputMaybe<Scalars['Int']['input']>;
};


/** The app queries. */
export type ApplicationQueriesQueryAssetsWithTotalArgs = {
  filter?: InputMaybe<Scalars['String']['input']>;
  orderby?: InputMaybe<Scalars['String']['input']>;
  skip?: InputMaybe<Scalars['Int']['input']>;
  top?: InputMaybe<Scalars['Int']['input']>;
};


/** The app queries. */
export type ApplicationQueriesQueryContentsByIdsArgs = {
  ids: Array<Scalars['String']['input']>;
};


/** The app queries. */
export type ApplicationQueriesQueryPagesContentsArgs = {
  collation?: InputMaybe<Scalars['String']['input']>;
  filter?: InputMaybe<Scalars['String']['input']>;
  orderby?: InputMaybe<Scalars['String']['input']>;
  random?: InputMaybe<Scalars['Int']['input']>;
  search?: InputMaybe<Scalars['String']['input']>;
  skip?: InputMaybe<Scalars['Int']['input']>;
  top?: InputMaybe<Scalars['Int']['input']>;
};


/** The app queries. */
export type ApplicationQueriesQueryPagesContentsWithTotalArgs = {
  collation?: InputMaybe<Scalars['String']['input']>;
  filter?: InputMaybe<Scalars['String']['input']>;
  orderby?: InputMaybe<Scalars['String']['input']>;
  random?: InputMaybe<Scalars['Int']['input']>;
  search?: InputMaybe<Scalars['String']['input']>;
  skip?: InputMaybe<Scalars['Int']['input']>;
  top?: InputMaybe<Scalars['Int']['input']>;
};


/** The app queries. */
export type ApplicationQueriesQueryPostsContentsArgs = {
  collation?: InputMaybe<Scalars['String']['input']>;
  filter?: InputMaybe<Scalars['String']['input']>;
  orderby?: InputMaybe<Scalars['String']['input']>;
  random?: InputMaybe<Scalars['Int']['input']>;
  search?: InputMaybe<Scalars['String']['input']>;
  skip?: InputMaybe<Scalars['Int']['input']>;
  top?: InputMaybe<Scalars['Int']['input']>;
};


/** The app queries. */
export type ApplicationQueriesQueryPostsContentsWithTotalArgs = {
  collation?: InputMaybe<Scalars['String']['input']>;
  filter?: InputMaybe<Scalars['String']['input']>;
  orderby?: InputMaybe<Scalars['String']['input']>;
  random?: InputMaybe<Scalars['Int']['input']>;
  search?: InputMaybe<Scalars['String']['input']>;
  skip?: InputMaybe<Scalars['Int']['input']>;
  top?: InputMaybe<Scalars['Int']['input']>;
};

export type ApplicationSubscriptions = {
  __typename?: 'ApplicationSubscriptions';
  /** Subscribe to asset events. */
  assetChanges?: Maybe<EnrichedAssetEvent>;
  /** Subscribe to content events. */
  contentChanges?: Maybe<EnrichedContentEvent>;
};


export type ApplicationSubscriptionsAssetChangesArgs = {
  type?: InputMaybe<EnrichedAssetEventType>;
};


export type ApplicationSubscriptionsContentChangesArgs = {
  schemaName?: InputMaybe<Scalars['String']['input']>;
  type?: InputMaybe<EnrichedContentEventType>;
};

/** An asset */
export type Asset = {
  __typename?: 'Asset';
  /** The timestamp when the object was created. */
  created: Scalars['Instant']['output'];
  /** The user who created the object. */
  createdBy: Scalars['String']['output'];
  /** The user who created the object. */
  createdByUser: User;
  /** The edit token. */
  editToken?: Maybe<Scalars['String']['output']>;
  /** The hash of the file. Can be null for old files. */
  fileHash: Scalars['String']['output'];
  /** The file name of the asset. */
  fileName: Scalars['String']['output'];
  /** The size of the file in bytes. */
  fileSize: Scalars['Int']['output'];
  /** The file type (file extension) of the asset. */
  fileType: Scalars['String']['output'];
  /** The version of the file. */
  fileVersion: Scalars['Int']['output'];
  /** The ID of the object (usually GUID). */
  id: Scalars['String']['output'];
  /**
   * Determines if the uploaded file is an image.
   * @deprecated Use 'type' field instead.
   */
  isImage: Scalars['Boolean']['output'];
  /** True, when the asset is not public. */
  isProtected: Scalars['Boolean']['output'];
  /** The timestamp when the object was updated the last time. */
  lastModified: Scalars['Instant']['output'];
  /** The user who updated the object the last time. */
  lastModifiedBy: Scalars['String']['output'];
  /** The user who updated the object the last time. */
  lastModifiedByUser: User;
  /** The asset metadata. */
  metadata?: Maybe<Scalars['JsonScalar']['output']>;
  /** The type of the image. */
  metadataText: Scalars['String']['output'];
  /** The mime type. */
  mimeType: Scalars['String']['output'];
  /** The ID of the parent folder. Empty for files without parent. */
  parentId: Scalars['String']['output'];
  /**
   * The height of the image in pixels if the asset is an image.
   * @deprecated Use 'metadata' field instead.
   */
  pixelHeight?: Maybe<Scalars['Int']['output']>;
  /**
   * The width of the image in pixels if the asset is an image.
   * @deprecated Use 'metadata' field instead.
   */
  pixelWidth?: Maybe<Scalars['Int']['output']>;
  /** The file name as slug. */
  slug: Scalars['String']['output'];
  /** The source URL of the asset. */
  sourceUrl?: Maybe<Scalars['String']['output']>;
  /** The asset tags. */
  tags: Array<Scalars['String']['output']>;
  /** The thumbnail URL to the asset. */
  thumbnailUrl?: Maybe<Scalars['String']['output']>;
  /** The type of the asset. */
  type: AssetType;
  /** The URL to the asset. */
  url: Scalars['String']['output'];
  /** The version of the objec. */
  version: Scalars['Int']['output'];
};


/** An asset */
export type AssetMetadataArgs = {
  path?: InputMaybe<Scalars['String']['input']>;
};

/** List of assets and total count of assets. */
export type AssetResultDto = {
  __typename?: 'AssetResultDto';
  /** The assets. */
  items: Array<Asset>;
  /** The total count of assets. */
  total: Scalars['Int']['output'];
};

export const AssetType = {
  Audio: 'AUDIO',
  Image: 'IMAGE',
  Unknown: 'UNKNOWN',
  Video: 'VIDEO'
} as const;

export type AssetType = typeof AssetType[keyof typeof AssetType];
/** The structure of all content types. */
export type Component = {
  /** schemaId */
  schemaId: Scalars['String']['output'];
  /** schemaName */
  schemaName?: Maybe<Scalars['String']['output']>;
};

/** The structure of all content types. */
export type Content = {
  /** created */
  created: Scalars['Instant']['output'];
  /** createdBy */
  createdBy: Scalars['String']['output'];
  /** data__dynamic */
  data__dynamic?: Maybe<Scalars['JsonScalar']['output']>;
  /** editToken */
  editToken?: Maybe<Scalars['String']['output']>;
  /** id */
  id: Scalars['String']['output'];
  /** lastModified */
  lastModified: Scalars['Instant']['output'];
  /** lastModifiedBy */
  lastModifiedBy: Scalars['String']['output'];
  /** newStatus */
  newStatus?: Maybe<Scalars['String']['output']>;
  /** newStatusColor */
  newStatusColor?: Maybe<Scalars['String']['output']>;
  /** status */
  status: Scalars['String']['output'];
  /** statusColor */
  statusColor: Scalars['String']['output'];
  /** url */
  url: Scalars['String']['output'];
  /** version */
  version: Scalars['Int']['output'];
};

/** An asset event */
export type EnrichedAssetEvent = {
  __typename?: 'EnrichedAssetEvent';
  /** The type of the asset. */
  assetType: AssetType;
  /** The timestamp when the object was created. */
  created: Scalars['Instant']['output'];
  /** The user who created the object. */
  createdBy: Scalars['String']['output'];
  /** The user who created the object. */
  createdByUser: User;
  /** The hash of the file. Can be null for old files. */
  fileHash: Scalars['String']['output'];
  /** The file name of the asset. */
  fileName: Scalars['String']['output'];
  /** The size of the file in bytes. */
  fileSize: Scalars['Int']['output'];
  /** The file type (file extension) of the asset. */
  fileType: Scalars['String']['output'];
  /** The version of the file. */
  fileVersion: Scalars['Int']['output'];
  /** The ID of the object (usually GUID). */
  id: Scalars['String']['output'];
  /**
   * Determines if the uploaded file is an image.
   * @deprecated Use 'type' field instead.
   */
  isImage: Scalars['Boolean']['output'];
  /** True, when the asset is not public. */
  isProtected: Scalars['Boolean']['output'];
  /** The timestamp when the object was updated the last time. */
  lastModified: Scalars['Instant']['output'];
  /** The user who updated the object the last time. */
  lastModifiedBy: Scalars['String']['output'];
  /** The user who updated the object the last time. */
  lastModifiedByUser: User;
  /** The asset metadata. */
  metadata?: Maybe<Scalars['JsonScalar']['output']>;
  /** The mime type. */
  mimeType: Scalars['String']['output'];
  /**
   * The height of the image in pixels if the asset is an image.
   * @deprecated Use 'metadata' field instead.
   */
  pixelHeight?: Maybe<Scalars['Int']['output']>;
  /**
   * The width of the image in pixels if the asset is an image.
   * @deprecated Use 'metadata' field instead.
   */
  pixelWidth?: Maybe<Scalars['Int']['output']>;
  /** The file name as slug. */
  slug: Scalars['String']['output'];
  /** The source URL of the asset. */
  sourceUrl: Scalars['String']['output'];
  /** The thumbnail URL to the asset. */
  thumbnailUrl?: Maybe<Scalars['String']['output']>;
  /** The type of the event. */
  type?: Maybe<EnrichedAssetEventType>;
  /** The URL to the asset. */
  url: Scalars['String']['output'];
  /** The version of the objec. */
  version: Scalars['Int']['output'];
};


/** An asset event */
export type EnrichedAssetEventMetadataArgs = {
  path?: InputMaybe<Scalars['String']['input']>;
};

export const EnrichedAssetEventType = {
  Annotated: 'ANNOTATED',
  Created: 'CREATED',
  Deleted: 'DELETED',
  Updated: 'UPDATED'
} as const;

export type EnrichedAssetEventType = typeof EnrichedAssetEventType[keyof typeof EnrichedAssetEventType];
/** An content event */
export type EnrichedContentEvent = {
  __typename?: 'EnrichedContentEvent';
  /** The timestamp when the object was created. */
  created: Scalars['Instant']['output'];
  /** The user who created the object. */
  createdBy: Scalars['String']['output'];
  /** The user who created the object. */
  createdByUser: User;
  /** The data of the content. */
  data?: Maybe<Scalars['JsonScalar']['output']>;
  /** The previous data of the content. */
  dataOld?: Maybe<Scalars['JsonScalar']['output']>;
  /** The ID of the object (usually GUID). */
  id: Scalars['String']['output'];
  /** The timestamp when the object was updated the last time. */
  lastModified: Scalars['Instant']['output'];
  /** The user who updated the object the last time. */
  lastModifiedBy: Scalars['String']['output'];
  /** The user who updated the object the last time. */
  lastModifiedByUser: User;
  /** The new status of the content. */
  newStatus?: Maybe<Scalars['String']['output']>;
  /** The status of the content. */
  status: Scalars['String']['output'];
  /** The type of the event. */
  type?: Maybe<EnrichedContentEventType>;
  /** The version of the objec. */
  version: Scalars['Int']['output'];
};

export const EnrichedContentEventType = {
  Created: 'CREATED',
  Deleted: 'DELETED',
  Published: 'PUBLISHED',
  ReferenceUpdated: 'REFERENCE_UPDATED',
  StatusChanged: 'STATUS_CHANGED',
  Unpublished: 'UNPUBLISHED',
  Updated: 'UPDATED'
} as const;

export type EnrichedContentEventType = typeof EnrichedContentEventType[keyof typeof EnrichedContentEventType];
/** The result of a mutation */
export type EntitySavedResultDto = {
  __typename?: 'EntitySavedResultDto';
  /** The new version of the item. */
  version: Scalars['Long']['output'];
};

/** The structure of a pages content type. */
export type Pages = Content & {
  __typename?: 'Pages';
  /** The timestamp when the object was created. */
  created: Scalars['Instant']['output'];
  /** The user who created the object. */
  createdBy: Scalars['String']['output'];
  /** The user who created the object. */
  createdByUser: User;
  /** The data of the content. */
  data: PagesDataDto;
  /** The data of the content. */
  data__dynamic?: Maybe<Scalars['JsonScalar']['output']>;
  /** The edit token. */
  editToken?: Maybe<Scalars['String']['output']>;
  /** The flat data of the content. */
  flatData: PagesFlatDataDto;
  /** The ID of the object (usually GUID). */
  id: Scalars['String']['output'];
  /** The timestamp when the object was updated the last time. */
  lastModified: Scalars['Instant']['output'];
  /** The user who updated the object the last time. */
  lastModifiedBy: Scalars['String']['output'];
  /** The user who updated the object the last time. */
  lastModifiedByUser: User;
  /** The new status of the content. */
  newStatus?: Maybe<Scalars['String']['output']>;
  /** The status color of the content. */
  newStatusColor?: Maybe<Scalars['String']['output']>;
  /** The status of the content. */
  status: Scalars['String']['output'];
  /** The status color of the content. */
  statusColor: Scalars['String']['output'];
  /** The URL to the content. */
  url: Scalars['String']['output'];
  /** The version of the objec. */
  version: Scalars['Int']['output'];
};

/** The structure of the pages data type. */
export type PagesDataDto = {
  __typename?: 'PagesDataDto';
  slug?: Maybe<PagesDataSlugDto>;
  text?: Maybe<PagesDataTextDto>;
  title?: Maybe<PagesDataTitleDto>;
};

/** The structure of the pages data input type. */
export type PagesDataInputDto = {
  slug?: InputMaybe<PagesDataSlugInputDto>;
  text?: InputMaybe<PagesDataTextInputDto>;
  title?: InputMaybe<PagesDataTitleInputDto>;
};

/** The structure of the Slug field of the pages content type. */
export type PagesDataSlugDto = {
  __typename?: 'PagesDataSlugDto';
  iv?: Maybe<Scalars['String']['output']>;
};

/** The structure of the Slug field of the pages content input type. */
export type PagesDataSlugInputDto = {
  iv?: InputMaybe<Scalars['String']['input']>;
};

/** The structure of the Text field of the pages content type. */
export type PagesDataTextDto = {
  __typename?: 'PagesDataTextDto';
  iv?: Maybe<Scalars['String']['output']>;
};

/** The structure of the Text field of the pages content input type. */
export type PagesDataTextInputDto = {
  iv?: InputMaybe<Scalars['String']['input']>;
};

/** The structure of the Title field of the pages content type. */
export type PagesDataTitleDto = {
  __typename?: 'PagesDataTitleDto';
  iv?: Maybe<Scalars['String']['output']>;
};

/** The structure of the Title field of the pages content input type. */
export type PagesDataTitleInputDto = {
  iv?: InputMaybe<Scalars['String']['input']>;
};

/** The structure of the flat pages data type. */
export type PagesFlatDataDto = {
  __typename?: 'PagesFlatDataDto';
  slug?: Maybe<Scalars['String']['output']>;
  text?: Maybe<Scalars['String']['output']>;
  title?: Maybe<Scalars['String']['output']>;
};

/** List of pages items and total count. */
export type PagesResultDto = {
  __typename?: 'PagesResultDto';
  /** The contents. */
  items?: Maybe<Array<Pages>>;
  /** The total count of  contents. */
  total: Scalars['Int']['output'];
};

/** The structure of a posts content type. */
export type Posts = Content & {
  __typename?: 'Posts';
  /** The timestamp when the object was created. */
  created: Scalars['Instant']['output'];
  /** The user who created the object. */
  createdBy: Scalars['String']['output'];
  /** The user who created the object. */
  createdByUser: User;
  /** The data of the content. */
  data: PostsDataDto;
  /** The data of the content. */
  data__dynamic?: Maybe<Scalars['JsonScalar']['output']>;
  /** The edit token. */
  editToken?: Maybe<Scalars['String']['output']>;
  /** The flat data of the content. */
  flatData: PostsFlatDataDto;
  /** The ID of the object (usually GUID). */
  id: Scalars['String']['output'];
  /** The timestamp when the object was updated the last time. */
  lastModified: Scalars['Instant']['output'];
  /** The user who updated the object the last time. */
  lastModifiedBy: Scalars['String']['output'];
  /** The user who updated the object the last time. */
  lastModifiedByUser: User;
  /** The new status of the content. */
  newStatus?: Maybe<Scalars['String']['output']>;
  /** The status color of the content. */
  newStatusColor?: Maybe<Scalars['String']['output']>;
  /** The status of the content. */
  status: Scalars['String']['output'];
  /** The status color of the content. */
  statusColor: Scalars['String']['output'];
  /** The URL to the content. */
  url: Scalars['String']['output'];
  /** The version of the objec. */
  version: Scalars['Int']['output'];
};

/** The structure of the posts data type. */
export type PostsDataDto = {
  __typename?: 'PostsDataDto';
  slug?: Maybe<PostsDataSlugDto>;
  text?: Maybe<PostsDataTextDto>;
  title?: Maybe<PostsDataTitleDto>;
};

/** The structure of the posts data input type. */
export type PostsDataInputDto = {
  slug?: InputMaybe<PostsDataSlugInputDto>;
  text?: InputMaybe<PostsDataTextInputDto>;
  title?: InputMaybe<PostsDataTitleInputDto>;
};

/** The structure of the Slug field of the posts content type. */
export type PostsDataSlugDto = {
  __typename?: 'PostsDataSlugDto';
  iv?: Maybe<Scalars['String']['output']>;
};

/** The structure of the Slug field of the posts content input type. */
export type PostsDataSlugInputDto = {
  iv?: InputMaybe<Scalars['String']['input']>;
};

/** The structure of the Text field of the posts content type. */
export type PostsDataTextDto = {
  __typename?: 'PostsDataTextDto';
  iv?: Maybe<Scalars['String']['output']>;
};

/** The structure of the Text field of the posts content input type. */
export type PostsDataTextInputDto = {
  iv?: InputMaybe<Scalars['String']['input']>;
};

/** The structure of the Title field of the posts content type. */
export type PostsDataTitleDto = {
  __typename?: 'PostsDataTitleDto';
  iv?: Maybe<Scalars['String']['output']>;
};

/** The structure of the Title field of the posts content input type. */
export type PostsDataTitleInputDto = {
  iv?: InputMaybe<Scalars['String']['input']>;
};

/** The structure of the flat posts data type. */
export type PostsFlatDataDto = {
  __typename?: 'PostsFlatDataDto';
  slug?: Maybe<Scalars['String']['output']>;
  text?: Maybe<Scalars['String']['output']>;
  title?: Maybe<Scalars['String']['output']>;
};

/** List of posts items and total count. */
export type PostsResultDto = {
  __typename?: 'PostsResultDto';
  /** The contents. */
  items?: Maybe<Array<Posts>>;
  /** The total count of  contents. */
  total: Scalars['Int']['output'];
};

/** A user that created or modified a content or asset. */
export type User = {
  __typename?: 'User';
  /** The display name of this user. */
  displayName?: Maybe<Scalars['String']['output']>;
  /** The email address ofthis  user. */
  email?: Maybe<Scalars['String']['output']>;
  /** The ID of this user. */
  id: Scalars['String']['output'];
};

export type PageQueryQueryVariables = Exact<{ [key: string]: never; }>;


export type PageQueryQuery = { __typename?: 'ApplicationQueries', queryPagesContentsWithTotal?: { __typename?: 'PagesResultDto', total: number, items?: Array<{ __typename?: 'Pages', id: string, flatData: { __typename?: 'PagesFlatDataDto', title?: string | null, text?: string | null } }> | null } | null };


export const PageQueryDocument = {"kind":"Document","definitions":[{"kind":"OperationDefinition","operation":"query","name":{"kind":"Name","value":"pageQuery"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"queryPagesContentsWithTotal"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"total"}},{"kind":"Field","name":{"kind":"Name","value":"items"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"id"}},{"kind":"Field","name":{"kind":"Name","value":"flatData"},"selectionSet":{"kind":"SelectionSet","selections":[{"kind":"Field","name":{"kind":"Name","value":"title"}},{"kind":"Field","name":{"kind":"Name","value":"text"}}]}}]}}]}}]}}]} as unknown as DocumentNode<PageQueryQuery, PageQueryQueryVariables>;