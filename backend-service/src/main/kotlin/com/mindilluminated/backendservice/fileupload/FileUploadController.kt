package com.mindilluminated.backendservice.fileupload

import org.springframework.web.bind.annotation.PostMapping
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RequestParam
import org.springframework.web.bind.annotation.RestController
import org.springframework.web.multipart.MultipartFile

@RestController
@RequestMapping("/upload-file")
class FileUploadController(private val fileStorage: FileStorage) {

    @PostMapping
    fun uploadFile(@RequestParam("file") file: MultipartFile) {
        fileStorage.storeFile(file)
    }

}