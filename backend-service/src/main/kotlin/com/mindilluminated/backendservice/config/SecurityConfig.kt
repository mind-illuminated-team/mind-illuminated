package com.mindilluminated.backendservice.config

import org.springframework.context.annotation.Configuration
import org.springframework.security.config.annotation.web.builders.HttpSecurity
import org.springframework.security.config.annotation.web.configuration.WebSecurityConfigurerAdapter

@Configuration
class SecurityConfig : WebSecurityConfigurerAdapter() {

    override fun configure(http: HttpSecurity?) {
        http?.authorizeRequests()?.
                antMatchers("/actuator/**")?.permitAll()?.
                anyRequest()?.authenticated()?.
                and()?.
                oauth2Login()
    }

}