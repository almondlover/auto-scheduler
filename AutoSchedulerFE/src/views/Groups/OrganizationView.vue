<script setup lang="ts">
import type { Organization } from '@/classes/group';
import OrganizationCard from '@/components/OrganizationCard.vue';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { computed, onBeforeMount, onMounted, onUpdated, ref, watch, type Ref } from 'vue';
import { useRoute } from 'vue-router';

const groupStore = useGroupStore();
const {currentOrganizationIdx} = storeToRefs(groupStore);
const route = useRoute();

let organizationId = route.params.id ?? currentOrganizationIdx;
//let organization:Ref<Organization> = ref(groupStore.organization(parseInt(organizationId[0])).value ?? );

onMounted(()=>{
  groupStore.getOrganizaton(parseInt(organizationId[0]));
});
//update when current idx changes
watch(currentOrganizationIdx, ()=>{
  groupStore.getOrganizaton(currentOrganizationIdx.value);
});


const targetOrganization = computed(()=>{
    return groupStore.organization(!Number.isNaN(parseInt(route.params.id[0]))?parseInt(route.params.id[0]):currentOrganizationIdx.value).value ?? {
      id: 0,
      name: '',
      description: undefined,
      groups: [],
      members: [],
      activities: [],
      halls:[]
    }
  }
);
</script>

<template>
  <main>
    <OrganizationCard :organization="targetOrganization"></OrganizationCard>
  </main>
</template>