<script setup lang="ts">
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted, watch } from 'vue';
import Sheet from './ui/sheet/Sheet.vue';
import SheetTrigger from './ui/sheet/SheetTrigger.vue';
import Button from './ui/button/Button.vue';
import SheetContent from './ui/sheet/SheetContent.vue';
import SheetTitle from './ui/sheet/SheetTitle.vue';
import { useUserStore } from '@/stores/userStore';
import { onBeforeRouteUpdate } from 'vue-router';

const groupStore = useGroupStore();
const {currentOrganizationIdx, organizations} = storeToRefs(groupStore);

const userStore=useUserStore();
const { token, currentUser } = storeToRefs(userStore);

onMounted(()=>{
    groupStore.getOrganizatons();
    token.value=localStorage.getItem('userToken')??'';
})

watch(currentUser, ()=>{
    groupStore.getOrganizatons();
})
</script>

<template>
  <header class="bg-heavy text-white">
    <Sheet class="bg-secondary">
      <SheetTrigger class="text-lg font-bold">
          Organization
      </SheetTrigger>
      <SheetContent class="p-5 bg-secondary text-m font-semibold" side="left">
        <SheetTitle class="text-2xl font-bold">Organization links</SheetTitle>
        <RouterLink to="/organization">Organization page</RouterLink>
        <RouterLink to="/activities">Activities</RouterLink>
        <RouterLink to="/activities/halls">Halls</RouterLink>
        <RouterLink to="/groups/members">Members</RouterLink>
      </SheetContent>
    </Sheet>
    <select class="font-bold text-lg" v-model="currentOrganizationIdx">
      <option class="text-lg text-heavy" v-for="organization in organizations" :value="organization.id">{{organization.name}}</option>
    </select>
    <nav class="border-box h-15 font-bold text-2xl bg-primary text-white flex items-center justify-end">
      <div class="flex gap-5 items-center justify-around w-150">
        <RouterLink class="hover:bg-heavy" to="/">Home</RouterLink>
        <RouterLink to="/timesheets">Dashboard</RouterLink>
        <RouterLink to="/groups">Groups</RouterLink>
        <RouterLink v-show="token===''" to="/login">Login</RouterLink>
        <Button class="font-bold text-2xl" v-show="token!==''" @click="userStore.logout" >Logout</Button>
      </div>
    </nav>
  </header>
</template>