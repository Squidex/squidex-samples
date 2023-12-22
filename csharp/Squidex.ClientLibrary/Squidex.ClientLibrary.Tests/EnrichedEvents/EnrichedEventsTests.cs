// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Newtonsoft.Json;
using Squidex.ClientLibrary.EnrichedEvents;

namespace Squidex.ClientLibrary.Tests.EnrichedEvents;

[UsesVerify]
public class EnrichedEventsTests
{

    private readonly SquidexOptions options = new SquidexOptions();

    public class SchemaData
    {
        [JsonConverter(typeof(InvariantConverter))]
        public string TestField { get; set; }
    }

    public class Schema : Content<SchemaData>
    {
    }

    private static readonly string JsonEnrichedContentEvent = @"{
		'type': 'SchemaUpdated',
		'payload': {
			'$type': 'EnrichedContentEvent',
			'id': '062b936f-7496-4f87-bd4f-ba7bbb63c751',
			'actor': 'subject:601c2cbafa4e669f214c0438',
			'appId': '3e2df825-86a9-43cb-8eb7-97d5a5bd4eea,testapp',
            'created': '2021-01-01T00:00:00Z',
			'createdBy': 'subject:601c2cbafa4e669f214c0438',
			'lastModified': '2021-01-01T00:01:00Z',
			'lastModifiedBy': 'subject:601c2cbafa4e669f214c0438',
			'name': 'SchemaUpdated',
			'partition': -792991992,
			'schemaId': '062b936f-7496-4f87-bd4f-ba7bbb63c751,schema',
            'status': 'Published',
			'timestamp': '2021-01-01T00:01:00Z',
			'type': 'Updated',
			'version': 3,
			'data': {
				'testField': {
					'iv': 'test2'
				}
			},
			'dataOld': {
				'testField': {
					'iv': 'test'
				}
			}
		},
		'timestamp': '2021-01-01T00:01:00Z'
	}";

    [Fact]
    public async void Should_deserialize_EnrichedContentEvent()
    {
        var envelope = EnrichedEventEnvelope.FromJson(JsonEnrichedContentEvent, options);

        Assert.True(envelope.Payload is EnrichedContentEvent);

        await Verify(envelope.Payload);
    }

    public static string JsonEnrichedCommentEvent { get; } = @"{
	    'type': 'UserMentioned',
	    'payload': {
		    '$type': 'EnrichedCommentEvent',
		    'actor': 'subject:601c2cbafa4e669f214c0438',
		    'appId': '3e2df825-86a9-43cb-8eb7-97d5a5bd4eea,testapp',
		    'name': 'UserMentioned',
		    'partition': -1730311374,
            'text': '@user@test.com testmessage',
		    'timestamp': '2021-01-01T00:00:00Z',
		    'url': '/app/testapp/content/schema/0e5955e3-cd2a-49f2-92ba-303acf4dd192/comments',
		    'version': 0
	    },
	    'timestamp': '2021-01-01T00:00:00Z'
    }";

    [Fact]
    public async Task Should_deserialize_EnrichedCommentEvent()
    {
        var envelope = EnrichedEventEnvelope.FromJson(JsonEnrichedCommentEvent, options);

        Assert.True(envelope.Payload is EnrichedCommentEvent);

        await Verify(envelope.Payload);
    }

    public static string JsonEnrichedAssetEvent { get; } = @"{
	    'type': 'AssetCreatedFromSnapshot',
	    'payload': {
		    '$type': 'EnrichedAssetEvent',
		    'id': 'c5dc4403-713d-4ebd-9a8f-a17efdba924e',
		    'actor': 'subject:6025a698a825d86becf541fe',
		    'appId': '3e2df825-86a9-43cb-8eb7-97d5a5bd4eea,testapp',
		    'assetType': 'Unknown',
		    'created': '2021-01-01T00:00:00Z',
		    'createdBy': 'subject:6025a698a825d86becf541fe',
		    'fileName': 'name.pdf',
		    'fileSize': 447021,
		    'fileVersion': 0,
		    'isImage': false,
		    'lastModified': '2021-01-01T00:00:00Z',
		    'lastModifiedBy': 'subject:6025a698a825d86becf541fe',
		    'mimeType': 'application/pdf',
		    'name': 'AssetCreatedFromSnapshot',
		    'partition': -755061617,
		    'timestamp': '1970-01-01T00:00:00Z',
            'type': 'Created',
		    'version': 1
	    },
	    'timestamp': '1970-01-01T00:00:00Z'
    }";

    [Fact]
    public async Task Should_deserialize_EnrichedAssetEvent()
    {
        var envelope = EnrichedEventEnvelope.FromJson(JsonEnrichedAssetEvent, options);

        Assert.True(envelope.Payload is EnrichedAssetEvent);

        await Verify(envelope.Payload);
    }
}
