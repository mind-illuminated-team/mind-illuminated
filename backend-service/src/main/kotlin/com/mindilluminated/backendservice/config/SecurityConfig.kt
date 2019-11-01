package com.mindilluminated.backendservice.config

import com.mindilluminated.backendservice.security.TokenAuthFilter
import org.springframework.beans.factory.annotation.Value
import org.springframework.boot.web.servlet.FilterRegistrationBean
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration

@Configuration
class SecurityConfig {

    @Value("\${header-token.name}")
    lateinit var headerTokenName: String

    @Value("\${header-token.value}")
    lateinit var headerTokenValue: String

    @Bean
    fun securityFilter() : FilterRegistrationBean<TokenAuthFilter> {
        return FilterRegistrationBean(TokenAuthFilter(headerTokenName, headerTokenValue))
    }

}