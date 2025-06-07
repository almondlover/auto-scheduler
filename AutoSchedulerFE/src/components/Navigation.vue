<script setup lang="ts">
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted } from 'vue';

const groupStore = useGroupStore();
const {currentOrganizationIdx, organizations} = storeToRefs(groupStore);

onMounted(()=>{
    groupStore.getOrganizatons();
})
</script>

<template>
  <header>
    <select v-model="currentOrganizationIdx">
      <option v-for="organization in organizations" :value="organization.id">{{organization.name}}</option>
    </select>
    <nav>
      <RouterLink to="/">Home</RouterLink>
      <RouterLink to="/about">About</RouterLink>
      <RouterLink to="/timesheets">Dashboard</RouterLink>
      <RouterLink to="/groups">Groups</RouterLink>
      <RouterLink to="/user">Profile</RouterLink>
    </nav>
  </header>
</template>