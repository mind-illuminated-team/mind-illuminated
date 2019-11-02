package com.mindilluminated.backendservice.security

import org.springframework.util.AntPathMatcher
import org.springframework.web.filter.OncePerRequestFilter
import javax.servlet.FilterChain
import javax.servlet.http.HttpServletRequest
import javax.servlet.http.HttpServletResponse

class AccessTokenFilter(private val securityService: SecurityService,
                        private val excludedPaths : List<String>) : OncePerRequestFilter() {

    companion object {
        const val ACCESS_TOKEN_HEADER = "X-Access-Token"
    }

    private val pathMatcher = AntPathMatcher()

    override fun doFilterInternal(request: HttpServletRequest, response: HttpServletResponse,
                                  filterChain: FilterChain) {

        var accessToken = request.getHeader(ACCESS_TOKEN_HEADER)

        if (securityService.hasAccess(accessToken)) {

            response.setHeader(ACCESS_TOKEN_HEADER, accessToken)
            filterChain.doFilter(request, response)

        } else {

            accessToken = securityService.refreshAccess(accessToken)

            if (accessToken != null) {

                response.setHeader(ACCESS_TOKEN_HEADER, accessToken)
                filterChain.doFilter(request, response)

            } else {

                response.status = 401

            }
        }
    }

    override fun shouldNotFilter(request: HttpServletRequest): Boolean {
        return excludedPaths.any { p -> pathMatcher.match(p, request.servletPath) }
    }
}