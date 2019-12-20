# Mind Illuminated Backend

The purpose of this backend application is to provide a REST API for the Android client application
for file uploading - this is required to persist the sensor data gathered during a session.

The application is using the Spring Boot framework, built with Gradle and was written in Kotlin.

## Build

Use Gradle to build the application: `gradle build`

## Deployment

Deployed to Heroku with HerokuCLI using `git subtree push --prefix backend-service heroku master`

## API Documentation

Available through the Swagger: http://mind-illuminated-backend.herokuapp.com/swagger-ui.html

## Features

### File upload

Google Drive is persistance storage for the uploaded files. This is achieved with the help of the official Google Drive API for Java.

Each client uploads a file after each session for later analysis of data.

### Authentication

The application is using OAuth to authenticate with the Android client application.
