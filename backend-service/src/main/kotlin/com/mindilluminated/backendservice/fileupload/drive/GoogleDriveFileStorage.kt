package com.mindilluminated.backendservice.fileupload.drive

import com.google.api.client.http.InputStreamContent
import com.google.api.services.drive.Drive
import com.google.api.services.drive.model.File
import com.google.api.services.drive.model.FileList
import com.mindilluminated.backendservice.fileupload.FileStorage
import com.mindilluminated.backendservice.security.SecurityUtils
import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Value
import org.springframework.stereotype.Service
import org.springframework.web.multipart.MultipartFile

@Service
class GoogleDriveFileStorage(private val drive: Drive) : FileStorage {

    companion object {
        val logger = LoggerFactory.getLogger(this::class.java)!!
        const val FOLDER_MIME_TYPE = "application/vnd.google-apps.folder"
    }

    @Value("\${google-drive.parent-folder-id}")
    lateinit var parentFolderId: String

    override fun storeFile(file: MultipartFile) {

        val email = SecurityUtils.getUserEmail()

        val folders = drive.files()
                .list()
                .setQ("mimeType='$FOLDER_MIME_TYPE' and '$parentFolderId' in parents")
                .execute() as FileList
        logger.info("Folders: {}", folders)

        val folderId = getFolderId(email, folders)

        if (folderId == null) {
            createFolder(email)
        }

        val driveFile = File()
        driveFile.name =  file.originalFilename
        driveFile.parents = listOf(folderId)
        driveFile.hasAugmentedPermissions = true

        val fileId = drive
                .files()
                .create(driveFile, InputStreamContent(file.contentType, file.inputStream))
                .setFields("id")
                .execute()
        logger.info("File stored in drive with id: {}", fileId)
    }

    fun getFolderId(name : String, folders : FileList) : String? {
        for (folder in folders.files) {
            if (folder["name"] == name) {
                return folder["id"] as String
            }
        }
        return null
    }

    fun createFolder(name : String) {
        val folder = File()
        folder.name = name
        folder.mimeType = FOLDER_MIME_TYPE
        folder.parents = listOf(parentFolderId)
        folder.hasAugmentedPermissions = true
        drive.files().create(folder).execute()
    }

}