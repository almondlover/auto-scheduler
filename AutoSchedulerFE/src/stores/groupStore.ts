import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import type { Group, Organization } from '@/classes/group';
import { fetchGroupsForOrganization, fetchOrganization, fetchOrganizations } from '@/services/groupService';

export const useGroupStore = defineStore('group', () => {
  const currentOrganizationIdx = ref(0);
  const organizations:Ref<Organization[]> = ref([]);
  const organization = (orgId:number)=>computed(()=>{return organizations.value.find(o=>o.id==orgId)});
  const groups:Ref<Group[]> = ref([]);
  const current = ref(0);
  const currentGroup = computed(()=>{return groups.value.find(g=>g.id==current.value)});
  async function getGroupsForOrganization(organizationId:number) {
    groups.value = await fetchGroupsForOrganization(organizationId);
  }
  async function getGroupsForCurrentOrganization() {
    groups.value = await fetchGroupsForOrganization(currentOrganizationIdx.value);
  }
  async function getOrganizaton(organizationId:number) {
    if (!organization(organizationId))
      organizations.value.push(await fetchOrganization(organizationId));
  }
  //should get per-user orgs instead
  async function getOrganizatons() {
    organizations.value = await fetchOrganizations();
  }
  return { currentOrganizationIdx, organizations, groups, current, currentGroup, organization, getGroupsForOrganization, getGroupsForCurrentOrganization, getOrganizatons, getOrganizaton }
});
