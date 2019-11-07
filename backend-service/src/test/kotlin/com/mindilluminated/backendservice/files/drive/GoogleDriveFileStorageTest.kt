package com.mindilluminated.backendservice.files.drive

import com.google.api.services.drive.Drive
import com.google.api.services.drive.model.File
import com.google.api.services.drive.model.FileList
import com.mindilluminated.backendservice.files.drive.GoogleDriveFileStorage.Companion.FOLDER_MIME_TYPE
import io.kotlintest.matchers.types.shouldNotBeNull
import io.kotlintest.shouldBe
import io.kotlintest.specs.BehaviorSpec
import io.mockk.every
import io.mockk.mockk
import io.mockk.spyk
import io.mockk.verify
import java.io.FileNotFoundException
import java.io.OutputStream

class GoogleDriveFileStorageTest : BehaviorSpec({

    Given("GoogleDriveFileStorage") {
        val drive = mockk<Drive>(relaxed = true)
        val fileStorage = spyk(GoogleDriveFileStorage(drive))

        val parentFolderId = "parentFolderId"
        fileStorage.parentFolderId = parentFolderId

        val returnedFile = File()
        returnedFile.id = "expectedFileId"

        every {
            drive.files().create(any(), any())
                    .setFields("id")
                    .execute()
        } returns returnedFile

        When("storing file") {

            val folderName = "testFolder"
            val folderList = FileList()
            val folder = File()
            folder.name = folderName
            folder.id = "testFolderId"
            folderList.files = listOf(folder)

            every {
                drive.files().list()
                        .setQ("mimeType='$FOLDER_MIME_TYPE' and '$parentFolderId' in parents")
                        .execute()
            } returns folderList

            val fileId = fileStorage.storeFile(folderName, mockk(relaxed = true))

            Then("the stored file's id should be returned") {
                fileId.shouldBe(returnedFile.id)
            }
        }

        When("storing file without an existing parent folder") {

            val notExistingFolderName = "notExistingTestFolder"
            every {
                drive.files().list()
                        .setQ("mimeType='$FOLDER_MIME_TYPE' and '$notExistingFolderName' in parents")
                        .execute()
            } returns FileList().setFiles(emptyList())

            val newFolder = File()
            newFolder.name = notExistingFolderName
            newFolder.id = "testFolderId"
            every {
                drive.files().create(any()).setFields("id").execute()
            } returns newFolder

            val fileId = fileStorage.storeFile(notExistingFolderName, mockk(relaxed = true))

            Then("the parent folder should be created") {
                verify { fileStorage.createFolder(notExistingFolderName) }
            }
            Then("the stored file's id should be returned") {
                fileId.shouldBe(returnedFile.id)
            }
        }

        When("downloading a file") {
            val outputStream = mockk<OutputStream>(relaxed = true)

            val fileList = FileList()
            val file = File()
            file.name = "testFile"
            file.id = "testFileId"
            fileList.files = listOf(file)
            every {
                drive.files()
                        .list()
                        .setQ("'$parentFolderId' in parents")
                        .setFields("*")
                        .execute()
            } returns fileList

            val downloadedFile = fileStorage.downloadFile(null, file.name, outputStream)
            Then("the downloaded file should be returned") {
                downloadedFile.id.shouldBe(file.id)
            }
            Then("the output stream should be filled") {
                verify { drive.files().get(file.id).executeMediaAndDownloadTo(outputStream) }
            }
        }

        When("downloading a file but it does not exist") {
            val outputStream = mockk<OutputStream>(relaxed = true)

            every {
                drive.files()
                        .list()
                        .setQ("'$parentFolderId' in parents")
                        .setFields("*")
                        .execute()
            } returns FileList().setFiles(emptyList())

            lateinit var exception : FileNotFoundException
            try {
                fileStorage.downloadFile(null, "notExistingFile", outputStream)
            } catch (e : FileNotFoundException) {
                exception = e
            }
            Then("a file not found exception should be thrown") {
                exception.shouldNotBeNull()
            }
        }

        When("listing files in a folder") {
            val fileList = FileList()
            val file = File()
            file.name = "testFile"
            file.id = "testFileId"
            fileList.files = listOf(file)
            every {
                drive.files()
                        .list()
                        .setQ("'$parentFolderId' in parents")
                        .setFields("*")
                        .execute()
            } returns fileList

            val list = fileStorage.getFiles(null)
            Then("the list of files in the folder should be returned") {
                list.shouldBe(fileList.files)
            }
        }
    }

})