import { SvelteKitAuth } from "@auth/sveltekit"
import Keycloak from "@auth/sveltekit/providers/keycloak"
import { services__keyauth__http__0 } from "$env/static/private"

export const { handle, signIn, signOut } = SvelteKitAuth({
    providers: [
        Keycloak({
            clientId: "ticksys-web",
            issuer: `${services__keyauth__http__0}/realms/ticksys`,
            clientSecret: "phcQhB07MbcsQuy9RUgCuzmqikF5jgQM",
        })
    ],
})