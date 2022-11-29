const hotel = `
    id,
    flatData {
        name,
        description,
        minPrice,
        rooms,
        photos {
            id,
            fileName
        }
    }`;

const destination = `
    id,
    flatData {
        name,
        description
    }`;

const post = `
    id
    flatData {
        title
        text {
            text
            contents {
                ...on Hotels {
                    ${hotel}
                }
            }
        }
    }`;

const landingPage = `
    id
    flatData {
        title
        content
        references {
            ...on Hotels {
                ${hotel}
            },
            ...on Destination {
                ${destination}
            }
        }
    }`;

export const GRAPHQL_FIELDS = { hotel, post, landingPage };