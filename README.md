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

## Article

- `/GET` should be accessible for all  
- `/POST` is only accessible for Writer  
- `/PUT` is accessible for Editor and only for Writer if `authorId` matches `userId`  
- `/DELETE` is accessible for Editor and only for Writer if `authorId` matches `userId`  

## Comment

- `/GET` is accessible for authenticated users  
- `/POST` is accessible for Subscriber  
- `/PUT` is accessible for Editor and only for Subscriber if `authorId` matches `userId`  
- `/DELETE` is accessible for Editor and only for Subscriber if `authorId` matches `userId`  
