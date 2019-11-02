package com.mindilluminated.backendservice.config

import com.google.api.client.googleapis.auth.oauth2.GoogleClientSecrets
import com.google.api.client.googleapis.util.Utils
import com.mindilluminated.backendservice.security.AccessTokenFilter
import com.mindilluminated.backendservice.security.SecurityService
import org.springframework.beans.factory.annotation.Value
import org.springframework.boot.web.servlet.FilterRegistrationBean
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import org.springframework.util.ResourceUtils
import java.io.InputStreamReader

@Configuration
class SecurityConfig {

    companion object {
        val swaggerPaths = listOf("/swagger-ui.html/**", "/swagger-ui.html#/**",
                "/webjars/springfox-swagger-ui/**", "/swagger-resources/**", "/v2/api-docs")
    }

    @Value("\${client-secret.json}")
    lateinit var clientSecretJson: String

    @Value("#{'\${security.excluded-paths:/security/access-token}'.split(',')}")
    lateinit var excludedPaths : MutableList<String>

    @Bean
    fun securityFilter(securityService: SecurityService) : FilterRegistrationBean<AccessTokenFilter> {
        excludedPaths.addAll(swaggerPaths)
        return FilterRegistrationBean(AccessTokenFilter(securityService, excludedPaths))
    }

    @Bean
    fun googleClientSecrets() : GoogleClientSecrets {
        val clientSecretInputStream = ResourceUtils.getFile(clientSecretJson).inputStream()
        return GoogleClientSecrets.load(Utils.getDefaultJsonFactory(), InputStreamReader(clientSecretInputStream))
    }

}