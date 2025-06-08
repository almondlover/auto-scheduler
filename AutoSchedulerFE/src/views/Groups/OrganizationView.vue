<script setup lang="ts">
import type { Organization } from '@/classes/group';
import OrganizationCard from '@/components/OrganizationCard.vue';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted, ref, type Ref } from 'vue';
import { useRoute } from 'vue-router';

const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);
const route = useRoute();

let organizationId = route.params.id ?? currentOrganizationIdx;
let organization:Ref<Organization> = ref(groupStore.organization(parseInt(organizationId[0])).value ?? {
  id: 0,
  name: '',
  description: undefined,
  groups: []
});

onMounted(()=>{
    groupStore.getOrganizaton(parseInt(organizationId[0]));
});

</script>

<template>
  <main>
    <OrganizationCard :organization="organization"></OrganizationCard>
  </main>
</template>