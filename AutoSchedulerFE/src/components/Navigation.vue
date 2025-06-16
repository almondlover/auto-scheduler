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
const { token } = storeToRefs(userStore);
</script>

<template>
  <header>
    <Sheet>
      <SheetTrigger>
          Organization
      </SheetTrigger>
      <SheetContent side="left">
        <SheetTitle>Organization links</SheetTitle>
        <RouterLink to="/organization">Organization page</RouterLink>
        <RouterLink to="/activities">Activities</RouterLink>
        <RouterLink to="/activities/halls">Halls</RouterLink>
        <RouterLink to="/groups/members">Members</RouterLink>
      </SheetContent>
    </Sheet>
    <select v-model="currentOrganizationIdx">
      <option v-for="organization in organizations" :value="organization.id">{{organization.name}}</option>
    </select>
    <nav>
      <RouterLink to="/">Home</RouterLink>
      <RouterLink to="/about">About</RouterLink>
      <RouterLink to="/timesheets">Dashboard</RouterLink>
      <RouterLink to="/groups">Groups</RouterLink>
      <RouterLink v-show="token===''" to="/login">Login</RouterLink>
      <Button v-show="token!==''" @click="userStore.logout" >Logout</Button>
    </nav>
  </header>
</template>