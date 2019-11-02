package com.mindilluminated.backendservice.files.drive

import com.google.api.client.googleapis.auth.oauth2.GoogleCredential
import com.google.api.client.googleapis.util.Utils
import com.google.api.services.drive.Drive
import com.google.api.services.drive.DriveScopes
import org.springframework.beans.factory.annotation.Value
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import org.springframework.util.ResourceUtils

@Configuration
class GoogleDriveConfiguration {

    @Value("\${google-drive.credentials}")
    lateinit var credentialsLocation: String

    @Value("\${spring.application.name}")
    lateinit var applicationName: String

    @Bean
    fun drive(): Drive {
        val credentialsInputStream = ResourceUtils.getFile(credentialsLocation).inputStream()

        val credentials = GoogleCredential.fromStream(credentialsInputStream).createScoped(DriveScopes.all())

        return Drive.Builder(Utils.getDefaultTransport(), Utils.getDefaultJsonFactory(), credentials)
                .setApplicationName(applicationName)
                .build()
    }

}