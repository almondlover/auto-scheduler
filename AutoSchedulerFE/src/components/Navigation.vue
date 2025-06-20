<script setup lang="ts">
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { onMounted } from 'vue';
import Sheet from './ui/sheet/Sheet.vue';
import SheetTrigger from './ui/sheet/SheetTrigger.vue';
import Button from './ui/button/Button.vue';
import SheetContent from './ui/sheet/SheetContent.vue';
import SheetTitle from './ui/sheet/SheetTitle.vue';
import { useUserStore } from '@/stores/userStore';

const groupStore = useGroupStore();
const {currentOrganizationIdx, organizations} = storeToRefs(groupStore);

onMounted(()=>{
    groupStore.getOrganizatons();
})

const userStore=useUserStore();
const { token, currentUser } = storeToRefs(userStore);
</script>

<template>
  <header class="bg-heavy text-white">
    <Sheet class="bg-secondary">
      <SheetTrigger>
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
    <select v-model="currentOrganizationIdx">
      <option class="text-heavy" v-for="organization in organizations" :value="organization.id">{{organization.name}}</option>
    </select>
    <nav class="border-box h-15 font-bold text-2xl bg-primary text-white flex items-center justify-end">
      <div class="flex gap-5 items-center justify-around w-150">
        <RouterLink class="hover:bg-heavy" to="/">Home</RouterLink>
        <RouterLink to="/about">About</RouterLink>
        <RouterLink to="/timesheets">Dashboard</RouterLink>
        <RouterLink to="/groups">Groups</RouterLink>
        <RouterLink v-show="token===''" to="/login">Login</RouterLink>
        <Button v-show="currentUser" @click="userStore.logout" >Logout</Button>
      </div>
    </nav>
  </header>
</template>