const isDevelopment = false;

const CONFIG = {
    url: 'http://localhost:3000',
    appName: 'test',
    clientId: 'test:default',
    clientSecret: 'mxfw5ybj3mq6euycdzfaa5kxh4bd9ntzmtc81za9u5ox'
};

if (isDevelopment) {
    CONFIG.url = 'http://localhost:5000';
}

$(() => {

    let auth = `${CONFIG.url}/identity-server/connect/token`;

    $.ajax({
        type: 'POST',
        url: auth, 
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        dataType: 'json',
        data: {
            grant_type: 'client_credentials',
            client_id: CONFIG.clientId,
            client_secret: CONFIG.clientSecret,
            scope: 'squidex-api'
        },
        success: authResponse => {
            let content = `${CONFIG.url}/api/content/${CONFIG.appName}/posts?$top = 10`

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
                        $('<li>').text(`REST: ${item.data.title.iv}`).appendTo($('#root'));
                    }
                }
            });

            let graphql = `${CONFIG.url}/api/content/${CONFIG.appName}/graphql`;

            const query = `
               query {
                queryPostsContents {
                  data {
                    title {
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
                    for (let item of itemsResponse.data.queryPostsContents) {
                        $('<li>').text(`GRAPHQL: ${item.data.title.iv}`).appendTo($('#root'));
                    }
                }
            });
        }
    });
});