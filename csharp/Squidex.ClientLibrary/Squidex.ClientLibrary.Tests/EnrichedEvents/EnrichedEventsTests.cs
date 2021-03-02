﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Squidex.ClientLibrary.EnrichedEvents;
using Xunit;

namespace Squidex.ClientLibrary.Tests.EnrichedEvents
{
    public class EnrichedEventsTests
    {
        private const string JsonEnrichedContentEvent = @"{
			'type': 'SchemaUpdated',
			'payload': {
				'$type': 'EnrichedContentEvent',
				'type': 'Updated',
				'id': '062b936f-7496-4f87-bd4f-ba7bbb63c751',
				'created': '2021-01-01T00:00:00Z',
				'lastModified': '2021-01-01T00:01:00Z',
				'createdBy': 'subject:601c2cbafa4e669f214c0438',
				'lastModifiedBy': 'subject:601c2cbafa4e669f214c0438',
				'data': {
					'testField': {
						'iv': 'test2'
					}
				},
				'dataOld': {
					'testField': {
						'iv': 'test'
					}
				},
				'status': 'Published',
				'partition': -792991992,
				'schemaId': '062b936f-7496-4f87-bd4f-ba7bbb63c751,schema',
				'actor': 'subject:601c2cbafa4e669f214c0438',
				'appId': '3e2df825-86a9-43cb-8eb7-97d5a5bd4eea,testapp',
				'timestamp': '2021-01-01T00:01:00Z',
				'name': 'SchemaUpdated',
				'version': 3
			},
			'timestamp': '2021-01-01T00:01:00Z'
		}";

        [Fact]
        public void Should_deserialize_EnrichedContentEvent()
        {
            var envelope = EnrichedEventEnvelope.DeserializeEnvelope(JsonEnrichedContentEvent);

            Assert.True(envelope.Payload is EnrichedContentEvent);

            var contentEvent = envelope.Payload as EnrichedContentEvent;

            Assert.Equal("SchemaUpdated", contentEvent.Name);
            Assert.Equal("testapp", contentEvent.App.Name);
            Assert.Equal("601c2cbafa4e669f214c0438", contentEvent.Actor.Id);
            Assert.Equal(typeof(JObject), contentEvent.Data.GetType());

            switch (contentEvent.Schema.Name)
            {
                case "schema":
                    var res = contentEvent.ToTyped<SchemaData>();

                    Assert.Equal(typeof(EnrichedContentEvent<SchemaData>), res.GetType());
                    Assert.Equal(typeof(SchemaData), res.Data.GetType());

                    Assert.Equal("test2", res.Data.TestField);
                    Assert.Equal("test", res.DataOld.TestField);
                    break;
                default: throw new Exception("Unknown schema");
            }
        }

        public static string JsonEnrichedCommentEvent { get; } = @"{
		    'type': 'UserMentioned',
		    'payload': {
			    '$type': 'EnrichedCommentEvent',
			    'text': '@user@test.com testmessage',
			    'url': '/app/testapp/content/schema/0e5955e3-cd2a-49f2-92ba-303acf4dd192/comments',
			    'partition': -1730311374,
			    'actor': 'subject:601c2cbafa4e669f214c0438',
			    'appId': '3e2df825-86a9-43cb-8eb7-97d5a5bd4eea,testapp',
			    'timestamp': '2021-01-01T00:00:00Z',
			    'name': 'UserMentioned',
			    'version': 0
		    },
		    'timestamp': '2021-01-01T00:00:00Z'
	    }";

        [Fact]
        public void Should_deserialize_EnrichedCommentEvent()
        {
            var envelope = EnrichedEventEnvelope.DeserializeEnvelope(JsonEnrichedCommentEvent);

            Assert.True(envelope.Payload is EnrichedCommentEvent);

            var contentEvent = envelope.Payload as EnrichedCommentEvent;

            Assert.Equal("@user@test.com testmessage", contentEvent.Text);
            Assert.Equal("UserMentioned", contentEvent.Name);
            Assert.Equal("testapp", contentEvent.App.Name);
            Assert.Equal("601c2cbafa4e669f214c0438", contentEvent.Actor.Id);
        }

        public static string JsonEnrichedAssetEvent { get; } = @"{
	            'type': 'AssetCreatedFromSnapshot',
	            'payload': {
		            '$type': 'EnrichedAssetEvent',
		            'type': 'Created',
		            'id': 'c5dc4403-713d-4ebd-9a8f-a17efdba924e',
		            'created': '2021-01-01T00:00:00Z',
		            'lastModified': '2021-01-01T00:00:00Z',
		            'createdBy': 'subject:6025a698a825d86becf541fe',
		            'lastModifiedBy': 'subject:6025a698a825d86becf541fe',
		            'mimeType': 'application/pdf',
		            'fileName': 'name.pdf',
		            'fileVersion': 0,
		            'fileSize': 447021,
		            'assetType': 'Unknown',
		            'isImage': false,
		            'partition': -755061617,
		            'actor': 'subject:6025a698a825d86becf541fe',
		            'appId': '3e2df825-86a9-43cb-8eb7-97d5a5bd4eea,testapp',
		            'timestamp': '1970-01-01T00:00:00Z',
		            'name': 'AssetCreatedFromSnapshot',
		            'version': 1
	            },
	            'timestamp': '1970-01-01T00:00:00Z'
            }";

        [Fact]
        public void Should_deserialize_EnrichedAssetEvent()
        {
            var envelope = EnrichedEventEnvelope.DeserializeEnvelope(JsonEnrichedAssetEvent);

            Assert.True(envelope.Payload is EnrichedAssetEvent);

            var contentEvent = envelope.Payload as EnrichedAssetEvent;

            Assert.Equal("AssetCreatedFromSnapshot", contentEvent.Name);
            Assert.Equal("testapp", contentEvent.App.Name);
            Assert.Equal("6025a698a825d86becf541fe", contentEvent.Actor.Id);
            Assert.Equal("c5dc4403-713d-4ebd-9a8f-a17efdba924e", contentEvent.Id);
            Assert.Equal(447021, contentEvent.FileSize);
            Assert.Equal("name.pdf", contentEvent.FileName);
            Assert.Equal("application/pdf", contentEvent.MimeType);
            Assert.Equal("6025a698a825d86becf541fe", contentEvent.LastModifiedBy.Id);
            Assert.Equal("subject", contentEvent.LastModifiedBy.Type);
            Assert.Equal("6025a698a825d86becf541fe", contentEvent.CreatedBy.Id);
            Assert.Equal("subject", contentEvent.CreatedBy.Type);
        }
    }

    public class SchemaData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string TestField { get; set; }
    }

    public class Schema : Content<SchemaData>
    {
    }
}
