package com.mindilluminated.backendservice

import io.kotlintest.specs.FreeSpec
import io.kotlintest.spring.SpringListener
import org.springframework.boot.test.context.SpringBootTest

@SpringBootTest
class BackendServiceApplicationTests : FreeSpec() {

    override fun listeners() = listOf(SpringListener)

    init {
        "Verify context loads" {}
    }

}
