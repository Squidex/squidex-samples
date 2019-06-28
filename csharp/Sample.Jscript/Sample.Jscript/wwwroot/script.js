const isDevelopment = false;

let host = 'https://cloud.squidex.io';
let clientId = 'client-test:client';
let clientSecret = 'Ify8nZ0O35OyZy6xAwxHFoYw5CcouaYyItPMpk1Df0o=';

if (isDevelopment) {
    host = 'http://localhost:5000';
}

$(() => {

    let auth = `${host}/identity-server/connect/token`;

    $.ajax({
        type: 'POST',
        url: auth,
        dataType: 'json',
        data: {
            grant_type: 'client_credentials',
            client_id: clientId,
            client_secret: clientSecret,
            scope: 'squidex-api'
        },
        success: authResponse => {

            let content = `${host}/api/content/client-test/numbers?$top=10`

            $.ajax({
                type: 'GET',
                url: content,
                dataType: 'json',
                data: {},
                headers: {
                    authorization: `Bearer ${authResponse.access_token}`
                },
                success: itemsResponse => {
                    for (let item of itemsResponse.items) {
                        $('<li>').text(`REST: ${item.data.value.iv}`).appendTo($('#root'));
                    }
                }
            });

            let graphql = `${host}/api/content/client-test/graphql`;

            const query = `
               query {
                queryNumbersContents {
                  data {
                    value {
                      iv
                    }
                  }
                }
              }`;

            $.ajax({
                type: 'POST',
                url: graphql,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: JSON.stringify({ query }),
                headers: {
                    authorization: `Bearer ${authResponse.access_token}`
                },
                success: itemsResponse => {
                    for (let item of itemsResponse.data.queryNumbersContents) {
                        $('<li>').text(`GRAPHQL: ${item.data.value.iv}`).appendTo($('#root'));
                    }
                }
            });
        }
    });
});