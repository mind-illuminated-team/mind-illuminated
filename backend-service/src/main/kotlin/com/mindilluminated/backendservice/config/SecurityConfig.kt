package com.mindilluminated.backendservice.config

import com.mindilluminated.backendservice.security.TokenAuthFilter
import org.springframework.beans.factory.annotation.Value
import org.springframework.context.annotation.Configuration
import org.springframework.security.config.annotation.web.builders.HttpSecurity
import org.springframework.security.config.annotation.web.configuration.WebSecurityConfigurerAdapter
import org.springframework.security.oauth2.client.web.OAuth2LoginAuthenticationFilter

@Configuration
class SecurityConfig : WebSecurityConfigurerAdapter() {

    @Value("\${header-token.name}")
    lateinit var headerTokenName: String

    @Value("\${header-token.value}")
    lateinit var headerTokenValue: String

    override fun configure(http: HttpSecurity?) {
        http!!.authorizeRequests()
                .antMatchers("/actuator/**").permitAll()
                .anyRequest().authenticated()
                .and()
                .csrf().disable()
                .oauth2Login()

        http.addFilterBefore(TokenAuthFilter(headerTokenName, headerTokenValue),
                OAuth2LoginAuthenticationFilter::class.java)
    }

}