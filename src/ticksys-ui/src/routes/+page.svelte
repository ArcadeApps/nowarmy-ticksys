<script lang="ts">
	import { page } from "$app/state"
	import { signIn } from '@auth/sveltekit/client';

</script>

<div>
	{#if page.data.session}
		<span>Logged In</span>
	{:else}
		<button onclick={() => signIn('keycloak')}>Login</button>
	{/if}
</div>