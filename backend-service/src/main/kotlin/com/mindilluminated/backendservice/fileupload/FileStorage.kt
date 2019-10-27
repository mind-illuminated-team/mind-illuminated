package com.mindilluminated.backendservice.fileupload

import org.springframework.web.multipart.MultipartFile

interface FileStorage {

    fun storeFile(file: MultipartFile)

}