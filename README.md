# Authorization start

Starter project for authorization workshop.

## Getting started

```sh
docker compose up --build
```

<http://localhost:5171/swagger/index.html>

Set `useCookies` and `useSessionCookies` to true for login.

# Authorization assignment

Initially, I tried to set up token-based authentication using JWT, but switched to using ASP.NET core Identity instead.

## Artice

/GET should be accessable for all
/POST is only accessable for Writer
/PUT is accessable for Editor and only for Writer if authorId matches userId
/DELETE is accessable for Editor and only for Writer if authorId matches userId

## Comment

/GET is accessable for authenticated users
/POST is accessable for Subscriber
/PUT is accessable for Editor and only for Subscriber if authorId matches userId
/DELETE is accessable for Editor and only for Subscriber if authorId matches userId
