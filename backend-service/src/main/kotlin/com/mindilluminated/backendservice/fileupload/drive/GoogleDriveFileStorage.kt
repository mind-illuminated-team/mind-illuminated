package com.mindilluminated.backendservice.fileupload.drive

import com.google.api.client.http.InputStreamContent
import com.google.api.services.drive.Drive
import com.google.api.services.drive.model.File
import com.google.api.services.drive.model.Permission
import com.mindilluminated.backendservice.fileupload.FileStorage
import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Value
import org.springframework.stereotype.Service
import org.springframework.web.multipart.MultipartFile

@Service
class GoogleDriveFileStorage(private val drive: Drive) : FileStorage {

    private val logger = LoggerFactory.getLogger(this::class.java)

    @Value("\${google-drive.parent-directory-id}")
    lateinit var parentDirectoryId: String

    override fun storeFile(file: MultipartFile) {
        val driveFile = File()
        driveFile.name =  file.originalFilename
        driveFile.parents = listOf(parentDirectoryId)
        driveFile.hasAugmentedPermissions = true

        logger.info("Files: {}", drive.files().list().execute())
        val fileId = drive
                .files()
                .create(driveFile, InputStreamContent(file.contentType, file.inputStream))
                .setFields("id")
                .execute()
        logger.info("File stored in drive with id: {}", fileId)
    }

}