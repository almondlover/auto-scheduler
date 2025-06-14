import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import type { Group, Member, Organization } from '@/classes/group';
import { createMember, deleteGroup, deleteMember, fetchGroupsForOrganization, fetchOrganization, fetchOrganizations, saveGroup } from '@/services/groupService';

export const useGroupStore = defineStore('group', () => {
  const currentOrganizationIdx = ref(0);
  const organizations:Ref<Organization[]> = ref([]);
  const organization = (orgId:number)=>computed(()=>{return organizations.value.find(o=>o.id==orgId)});
  const groups:Ref<Group[]> = ref([]);
  const members:Ref<Member[]> = ref([]);
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
  async function createGroup(group:Group) {
    let newGroup = saveGroup(group);
    groups.value.push(group);
  }
  async function saveMember(member:Member) {
    let newGroup = createMember(member);
    members.value.push(member);
  }
  async function removeGroup(groupId:number) {
    deleteGroup(groupId);
    groups.value.splice(groups.value.findIndex(group=>group.id===groupId), 1);
  }
  async function removeMember(memberId:number) {
    deleteMember(memberId);
    members.value.splice(members.value.findIndex(mem=>mem.id===memberId), 1);
  }
  return { currentOrganizationIdx, organizations, groups, current, currentGroup, members, organization, getGroupsForOrganization, getGroupsForCurrentOrganization, getOrganizatons, getOrganizaton, createGroup, saveMember, removeGroup, removeMember }
});
