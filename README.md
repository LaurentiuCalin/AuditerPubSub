# Publisher/Subscriber demo implementation

Can be run with Docker Compose with the following:

```
$ docker-compose -f docker-compose.yml -p pub-sub-demo up --build --force-recreate
```

The publisher is accessible through swagger on:

```
http://localhost:5003/swagger/index.html
```

Or with the following cURL:
```bash
curl -X 'POST' \
  'http://localhost:5003/User' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "name": {name},
  "email": {email}
}'
```

# Demo


https://user-images.githubusercontent.com/17986810/214096479-830c4e6a-6b7d-4de3-b62a-23ef616d85f9.mp4

