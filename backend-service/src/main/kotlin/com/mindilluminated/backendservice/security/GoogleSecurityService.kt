package com.mindilluminated.backendservice.security

import com.google.api.client.googleapis.auth.oauth2.*
import com.google.api.client.googleapis.util.Utils
import org.slf4j.LoggerFactory
import org.springframework.scheduling.annotation.Scheduled
import org.springframework.stereotype.Service
import java.time.LocalDateTime
import java.util.concurrent.ConcurrentHashMap

@Service
class GoogleSecurityService(private val googleClientSecrets: GoogleClientSecrets) : SecurityService {

    companion object {
        val logger = LoggerFactory.getLogger(this::class.java)!!
    }

    private final val tokenCache = ConcurrentHashMap<String, GoogleTokenResponse>()

    override fun getAccessToken(authCode: String) : String {
        val tokenResponse = GoogleAuthorizationCodeTokenRequest(
                Utils.getDefaultTransport(),
                Utils.getDefaultJsonFactory(),
                googleClientSecrets.details.clientId,
                googleClientSecrets.details.clientSecret,
                authCode,
                "")
                .execute()

        tokenResponse["expires"] = LocalDateTime.now().plusSeconds(tokenResponse.expiresInSeconds)

        tokenCache[tokenResponse.accessToken] = tokenResponse

        return tokenResponse.accessToken
    }

    override fun hasAccess(accessToken: String?): Boolean {
        if (accessToken == null) {
            return false
        }
        val token = tokenCache[accessToken] ?: return false

        val expires = token["expires"] as LocalDateTime

        return LocalDateTime.now().isBefore(expires)
    }

    override fun refreshAccess(accessToken: String?) : String? {
        if (accessToken == null) {
            return null
        }
        val token = tokenCache[accessToken] ?: return null

        val tokenResponse = GoogleRefreshTokenRequest(
                Utils.getDefaultTransport(),
                Utils.getDefaultJsonFactory(),
                token.refreshToken,
                googleClientSecrets.details.clientId,
                googleClientSecrets.details.clientSecret)
                .execute()

        tokenResponse["expires"] = LocalDateTime.now().plusSeconds(tokenResponse.expiresInSeconds)

        tokenCache.remove(accessToken)
        tokenCache[tokenResponse.accessToken] = tokenResponse

        return tokenResponse.accessToken
    }

    @Scheduled(cron = "\${token.scheduled.clear: 0 0 * * * ?}")
    fun removeExpiredTokens() {
        val now = LocalDateTime.now()
        tokenCache.entries.removeIf { entry -> now.isAfter(entry.value["expires"] as LocalDateTime) }
    }

}