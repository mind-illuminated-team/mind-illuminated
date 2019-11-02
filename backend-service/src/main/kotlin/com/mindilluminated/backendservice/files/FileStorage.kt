package com.mindilluminated.backendservice.files

import com.google.api.services.drive.model.File
import org.springframework.web.multipart.MultipartFile
import java.io.OutputStream

interface FileStorage {

    fun storeFile(folder : String?, file: MultipartFile)

    fun downloadFile(folder : String?, name : String, outputStream: OutputStream) : File

    fun getFiles(folder : String?) : List<File>

}