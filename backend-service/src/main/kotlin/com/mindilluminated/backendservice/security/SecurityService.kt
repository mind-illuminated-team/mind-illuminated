package com.mindilluminated.backendservice.security

interface SecurityService {

    fun getAccessToken(authCode : String) : String

    fun hasAccess(accessToken : String?) : Boolean

    fun refreshAccess(accessToken : String?) : String?

}