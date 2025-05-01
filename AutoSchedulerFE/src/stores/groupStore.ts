import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import type { Group } from '@/classes/group';
import { fetchGroupsForOrganization } from '@/services/groupService';

export const useGroupStore = defineStore('group', () => {
  const groups:Ref<Group[]> = ref([]);
  const current = ref(0);
  async function getGroupsForOrganization(organizationId:number) {
    groups.value = await fetchGroupsForOrganization(organizationId);
  }
  return { groups, current, getGroupsForOrganization }
})
