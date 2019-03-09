# SW Project Repository

### PostDto

```json
{
    "id": "UUID generated on server",
    "originalId": "1",
    "collection": "test",
    "text": "Text 1.",
    "keywords": [
        "key1",
        "key2"
    ]
}
```

### Pageable PostDto response
```json
{
    "count": 2,
    "pageNumber": 0,
    "pageSize": 2,
    "nextRequest": "/posts/test?pageNumber=1&pageSize=2",
    "data": [
        {
            "id": "22e01b73-015b-49ed-af54-ffb22a011f15",
            "originalId": "1",
            "collection": "test",
            "text": "Text 1.",
            "keywords": [
                "key1",
                "key2"
            ]
        },
        {
            "id": "e6bb468e-e133-4242-a779-e4f35750ba65",
            "originalId": "2",
            "collection": "test",
            "text": "Text 2.",
            "keywords": [
                "key2",
                "key3"
            ]
        }
    ]
}
```

### GET Mappings

- /post/{id}
- /posts/{collection}?pageNumber=0&pageSize=20
- /posts/{collection}?pageNumber=0&pageSize=20&text=text
- /posts/{collection}?pageNumber=0&pageSize=20&keyword=keyword

### POST PostDto Mappings

- /posts
- /post

## TODO

- analyzed posts repository
