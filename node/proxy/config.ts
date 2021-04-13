const config = {
    squidex: {
        url: trimEnd(process.env.SQUIDEX__URL || 'https://cloud.squidex.io'),
        clientId: process.env.SQUIDEX__CLIENTID || 'squidex-website:reader',
        clientSecret: process.env.SQUIDEX__CLIENTSECRET || 'yy9x4dcxsnp1s34r2z19t88wedbzxn1tfq7uzmoxf60x'
    },
    server: {
        port: parseInt(process.env.SERVER_PORT!) || 3000
    },
    cache: {
        duration: parseInt(process.env.CACHE__DURATION!) || (30 * 60),
    },
    redis: {
        url: process.env.REDIS__URL
    }
};

function trimEnd(value: string) {
    while (value.endsWith('/')) {
        value = value.substr(0, value.length - 1)
    }

    return value;
}

export default config;