package com.mindilluminated.backendservice.security

import org.springframework.web.filter.OncePerRequestFilter
import javax.servlet.FilterChain
import javax.servlet.http.HttpServletRequest
import javax.servlet.http.HttpServletResponse

class TokenAuthFilter(private val headerTokenName : String, private val headerTokenValue : String)
    : OncePerRequestFilter() {

    override fun doFilterInternal(request: HttpServletRequest, response: HttpServletResponse,
                                  filterChain: FilterChain) {
        val headerToken = request.getHeader(headerTokenName)

        if (headerTokenValue == headerToken) {
            filterChain.doFilter(request, response)
        } else {
            response.status = 401
        }
    }

}