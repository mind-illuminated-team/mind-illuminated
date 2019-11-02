package com.mindilluminated.backendservice.security

import com.mindilluminated.backendservice.security.AccessTokenFilter.Companion.ACCESS_TOKEN_HEADER
import org.springframework.web.bind.annotation.*
import javax.servlet.http.HttpServletResponse

@RestController
@RequestMapping("/security")
class SecurityController(private val securityService: SecurityService) {

    companion object {
        const val AUTH_HEADER_NAME = "X-Auth-Code"
    }

    @GetMapping("/access-token")
    fun accessToken(@RequestHeader(value = AUTH_HEADER_NAME, required = true) authCode : String,
                    response : HttpServletResponse) {
        val accessToken = securityService.getAccessToken(authCode)
        response.setHeader(ACCESS_TOKEN_HEADER, accessToken)
        response.status = 200
        response.flushBuffer()
    }

}