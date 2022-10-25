export default function slugify(...args) {
  // source: https://gist.github.com/codeguy/6684588?permalink_comment_id=3601100#gistcomment-3601100
  return args.join(' ')
    .normalize('NFD') // split an accented letter in the base letter and the accent
    .replace(/[\u0300-\u036f]/g, '') // remove all previously split accents
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9 ]/g, '') // remove all chars not letters, numbers and spaces (to be replaced)
    .replace(/\s+/g, '-'); // separator
}
