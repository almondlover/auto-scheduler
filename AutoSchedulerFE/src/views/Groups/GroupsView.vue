<script setup lang="ts">
import GroupForm from '@/components/GroupForm.vue';
import GroupList from '@/components/GroupList.vue';
import Button from '@/components/ui/button/Button.vue';
import { useGroupStore } from '@/stores/groupStore';
import { storeToRefs } from 'pinia';
import { ref } from 'vue';

const groupStore = useGroupStore();
const { groups, currentOrganizationIdx } = storeToRefs(groupStore);

const showNewActivityModal = ref(false);
</script>

<template>
  <main>
    <h1>Organization groups</h1>
    <Button @click="showNewActivityModal=!showNewActivityModal">New Group</Button>
    <GroupList/>
    <GroupForm v-show="showNewActivityModal" />
  </main>
</template>