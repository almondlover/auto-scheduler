import { ref, computed, type Ref } from 'vue'
import { defineStore } from 'pinia'
import type { Group, Member, Organization } from '@/classes/group';
import { createMember, deleteAvailability, deleteGroup, deleteMember, fetchGroup, fetchGroupsForOrganization, fetchOrganization, fetchOrganizations, fetchRootGroupsForOrganization, saveGroup, updateMember } from '@/services/groupService';

export const useGroupStore = defineStore('group', () => {
  const currentOrganizationIdx = ref(0);
  const organizations:Ref<Organization[]> = ref([]);
  const organization = (orgId:number)=>computed(()=>{return organizations.value?.find(o=>o.id==orgId)});
  const groups:Ref<Group[]> = ref([]);
  const members:Ref<Member[]> = ref([]);
  const current = ref(0);
  const currentGroup = computed(()=>{return groups.value.find(g=>g.id==current.value)});
  async function getGroupsForOrganization(organizationId:number) {
    groups.value = await fetchGroupsForOrganization(organizationId);
  }
  async function getRootGroupsForOrganization(organizationId:number) {
    groups.value = await fetchRootGroupsForOrganization(organizationId);
  }
  async function getGroupsForCurrentOrganization() {
    groups.value = await fetchGroupsForOrganization(currentOrganizationIdx.value);
  }
  async function getOrganizaton(organizationId:number) {
    if (!organization(organizationId))
      organizations.value.push(await fetchOrganization(organizationId));
  }
  async function getGroup(groupId:number) {
    if (!groups.value.some(grp=>grp.id==groupId))
      groups.value.push(await fetchGroup(groupId));
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
  async function modifyMember(member:Member) {
    updateMember(member);
    members.value.splice(members.value.findIndex(mem => mem.id===member.id), 1, member);
  }
  async function removeGroup(groupId:number) {
    deleteGroup(groupId);
    groups.value.splice(groups.value.findIndex(group=>group.id===groupId), 1);
  }
  async function removeMember(memberId:number) {
    deleteMember(memberId);
    members.value.splice(members.value.findIndex(mem=>mem.id===memberId), 1);
  }
  async function removeAvailability(availabilityId:number) {
    deleteAvailability(availabilityId);
    let memAvailIdx = -1;
    members.value.find(mem=>{
      memAvailIdx=mem.availability.findIndex(avail=>avail.id==availabilityId);
      return memAvailIdx!==-1})?.availability.splice(memAvailIdx, 1);
  }
  return { currentOrganizationIdx, organizations, groups, current, currentGroup, members, organization,
            getGroupsForOrganization, getGroupsForCurrentOrganization, getRootGroupsForOrganization, getOrganizatons, getOrganizaton, getGroup,
            createGroup, saveMember,
            modifyMember,
            removeGroup, removeMember, removeAvailability }
});
