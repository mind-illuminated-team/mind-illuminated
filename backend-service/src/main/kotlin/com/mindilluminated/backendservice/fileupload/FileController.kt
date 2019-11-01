package com.mindilluminated.backendservice.fileupload

import com.google.api.services.drive.model.File
import com.google.common.net.HttpHeaders
import org.springframework.http.ResponseEntity
import org.springframework.web.bind.annotation.*
import org.springframework.web.multipart.MultipartFile
import javax.servlet.http.HttpServletResponse

@RestController
@RequestMapping("/files")
class FileController(private val fileStorage: FileStorage) {

    @PostMapping("/upload")
    fun uploadFile(@RequestParam(value = "folder", required = false) folder : String?,
                   @RequestParam("file") file: MultipartFile) {
        fileStorage.storeFile(folder, file)
    }

    @GetMapping("/download")
    fun downloadFile(@RequestParam(value = "folder", required = false) folder : String?,
                     @RequestParam("name") name : String,
                     response : HttpServletResponse) {
        val file = fileStorage.downloadFile(folder, name, response.outputStream)
        response.setHeader(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=${file.name}")
        response.contentType = file.mimeType
        response.flushBuffer()
    }


    @GetMapping("/list")
    fun listFiles(@RequestParam(value = "folder", required = false) folder : String?) : ResponseEntity<List<File>> {
        return ResponseEntity.ok(fileStorage.getFiles(folder))
    }

}