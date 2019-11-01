package com.mindilluminated.backendservice.fileupload.drive

import com.google.api.client.http.InputStreamContent
import com.google.api.services.drive.Drive
import com.google.api.services.drive.model.File
import com.google.api.services.drive.model.FileList
import com.mindilluminated.backendservice.fileupload.FileStorage
import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Value
import org.springframework.stereotype.Service
import org.springframework.web.multipart.MultipartFile
import java.io.FileNotFoundException
import java.io.OutputStream

@Service
class GoogleDriveFileStorage(private val drive: Drive) : FileStorage {

    companion object {
        val logger = LoggerFactory.getLogger(this::class.java)!!
        const val FOLDER_MIME_TYPE = "application/vnd.google-apps.folder"
    }

    @Value("\${google-drive.parent-folder-id}")
    lateinit var parentFolderId: String

    override fun storeFile(folder : String?, file: MultipartFile) {
        val folderId = findFolderId(folder)

        if (folder != null && folderId == null) {
            createFolder(folder)
        }

        val parentId = if (folder == null) parentFolderId else folderId

        val driveFile = File()
        driveFile.name =  file.originalFilename
        driveFile.parents = listOf(parentId)
        driveFile.hasAugmentedPermissions = true

        val fileId = drive
                .files()
                .create(driveFile, InputStreamContent(file.contentType, file.inputStream))
                .setFields("id")
                .execute()
        logger.info("File stored in drive with id: {}", fileId)
    }

    override fun downloadFile(folder : String?, name: String, outputStream: OutputStream) : File {
        val folderId = findFolderId(folder)

        val file = getFile(name, listFiles(folderId ?: parentFolderId)) ?: throw FileNotFoundException()

        drive.files()
                .get(file["id"] as String)
                .executeMediaAndDownloadTo(outputStream)
        return file
    }

    override fun getFiles(folder: String?): List<File> {
        val folderId = findFolderId(folder)
        return listFiles(folderId ?: parentFolderId).files
    }

    fun listFiles(parent : String) : FileList {
        return drive.files()
                .list()
                .setQ("'$parent' in parents")
                .setFields("*")
                .execute()
    }

    fun findFolderId(name : String?) : String? {
        val folders = drive.files()
                .list()
                .setQ("mimeType='$FOLDER_MIME_TYPE' and '$parentFolderId' in parents")
                .execute() as FileList
        logger.debug("Folders: {}", folders)

        return getFile(name, folders)?.get("id") as String
    }

    fun getFile(name : String?, list : FileList?) : File? {
        if (list == null) {
            return null
        }
        for (file in list.files) {
            if (file["name"] == name) {
                return file
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