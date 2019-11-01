package com.mindilluminated.backendservice.security

import org.springframework.security.core.Authentication
import org.springframework.security.core.context.SecurityContextHolder
import org.springframework.security.oauth2.core.oidc.user.DefaultOidcUser

object SecurityUtils {

    private fun getAuthentication() : Authentication {
        return SecurityContextHolder.getContext().authentication
    }

    fun getUserEmail() : String {
        return (getAuthentication().principal as DefaultOidcUser).email
    }

}